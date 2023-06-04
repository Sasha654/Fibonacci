using PT.Fibonacci.Contract;
using System.Net.Http;
using System.Net.Http.Json;

namespace PT.Fibonacci.FirstApp;

[UsedImplicitly]
internal sealed class InitiatorService
{
    private readonly ILogger<InitiatorService> _logger;
    private readonly IRabbitClientFactory _rabbitClientFactory;
    private readonly HttpClient _httpClient;
    private string _calcRequestUri = string.Empty;
    private int _calculationId;
    private long _currentFibValue;

    public InitiatorService(
        ILogger<InitiatorService> logger,
        IRabbitClientFactory rabbitClientFactory,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _rabbitClientFactory = rabbitClientFactory;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task Run(string calcRequestUri, int calculationId, CancellationToken token)
    {
        _calcRequestUri = calcRequestUri;
        _calculationId = calculationId;

        _logger.LogInformation("ID [{CalculationId}] starting", calculationId);

        var rabbitConsumer = _rabbitClientFactory.CreateConsumer(calculationId);
        using var sub = rabbitConsumer.Consume<FibNumResponse>(response =>
        {
            _currentFibValue = response.Value;
            _logger.LogInformation("ID [{CalculationId}] fib num [{FibValue}]",
                _calculationId, _currentFibValue);
            return SendRequest(token);
        });

        await SendRequest(token);
        await Task.Delay(Timeout.InfiniteTimeSpan, token);
    }

    private async Task SendRequest(CancellationToken token)
    {
        var request = new FibNumRequest(_calculationId, _currentFibValue);
        var response = await _httpClient
            .PostAsJsonAsync(_calcRequestUri, request, token);

        if (response.IsSuccessStatusCode)
            _logger.LogDebug("ID [{CalculationId}] success sent request", _calculationId);
        else
            _logger.LogError("ID [{CalculationId}] fail sent request", _calculationId);
    }
}