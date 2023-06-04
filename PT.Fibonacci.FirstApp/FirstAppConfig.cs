namespace PT.Fibonacci.FirstApp;

[UsedImplicitly]
internal sealed class FirstAppConfig
{
    private const int DefaultParallelCalcCount = 1;
    private const string DefaultCalcRequestUri = @"http://localhost:5000/calculation";

    public int ParallelCalcCount { get; set; } = DefaultParallelCalcCount;

    public string CalcRequestUri { get; set; } = DefaultCalcRequestUri;
}