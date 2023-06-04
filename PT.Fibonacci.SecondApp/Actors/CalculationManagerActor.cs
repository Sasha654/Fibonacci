using System.Collections.Concurrent;

namespace PT.Fibonacci.SecondApp.Actors;

/// <summary>
/// Координатор запросов для расчета
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
internal sealed class CalculationManagerActor : ReceiveActor
{
    private const string WorkerPrefixName = "Worker_";
    private readonly ConcurrentDictionary<int, string> _workerNames = new();
    private readonly IRabbitClientFactory _rabbitClientFactory;

    public CalculationManagerActor(IRabbitClientFactory rabbitClientFactory)
    {
        _rabbitClientFactory = rabbitClientFactory;
        Receive<FibNumRequest>(OnNextFibNumRequest);
    }

    public static Props Props(IRabbitClientFactory rabbitClientFactory) =>
        Akka.Actor.Props.Create(() => new CalculationManagerActor(rabbitClientFactory));

    private void OnNextFibNumRequest(FibNumRequest request)
    {
        var workerName = GetOrCreateWorkerName(request.CalculationId);
        var worker = Context.Child(workerName);
        if (worker.Equals(ActorRefs.Nobody))
        {
            var rabbitProducer = _rabbitClientFactory.CreateProducer(request.CalculationId);
            worker = Context.ActorOf(CalculationWorkerActor.Props(rabbitProducer), workerName);
        }

        worker.Tell(request);
    }

    private string GetOrCreateWorkerName(int id) =>
        _workerNames.GetOrAdd(id, newId => WorkerPrefixName + newId);
}