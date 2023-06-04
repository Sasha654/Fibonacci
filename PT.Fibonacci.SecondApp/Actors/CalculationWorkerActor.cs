namespace PT.Fibonacci.SecondApp.Actors;

/// <summary>
/// Предназначен для расчета чисел
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
internal sealed class CalculationWorkerActor : ReceiveActor
{
    private readonly RabbitProducer _rabbitProducer;
    private long _previousValue = 1;

    public CalculationWorkerActor(RabbitProducer rabbitProducer)
    {
        _rabbitProducer = rabbitProducer;
    }

    public static Props Props(RabbitProducer rabbitProducer) =>
        Akka.Actor.Props.Create(() => new CalculationWorkerActor(rabbitProducer));

    protected override void PreStart()
    {
        Become(() => ReceiveAsync<FibNumRequest>(OnFirstRequest));
        base.PreStart();
    }

    private Task OnFirstRequest(FibNumRequest request)
    {
        Become(() => ReceiveAsync<FibNumRequest>(OnOtherRequest));
        var sum = _previousValue + request.Value;
        _previousValue = default;
        return SendResult(sum);
    }

    private Task OnOtherRequest(FibNumRequest request)
    {
        var sum = _previousValue + request.Value;
        _previousValue = request.Value;
        return SendResult(sum);
    }

    private Task SendResult(long value)
    {
        var response = new FibNumResponse(value);
        return _rabbitProducer.Produce(response);
    }
}