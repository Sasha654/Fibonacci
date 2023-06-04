using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PT.Fibonacci.SecondApp.Actors;

[UsedImplicitly]
internal sealed class AkkaService : IHostedService, IActorBridge
{
    private readonly IServiceProvider _serviceProvider;
    private ActorSystem? _actorSystem;
    private IActorRef? _calculationManager;

    public AkkaService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var bootstrap = BootstrapSetup.Create();
        var diSetup = DependencyResolverSetup.Create(_serviceProvider);
        var actorSystemSetup = bootstrap.And(diSetup);
        _actorSystem = ActorSystem.Create("akka-universe", actorSystemSetup);

        var rabbitClientFactory = _serviceProvider.GetRequiredService<IRabbitClientFactory>();
        _calculationManager = _actorSystem.ActorOf(
            CalculationManagerActor.Props(rabbitClientFactory), nameof(CalculationManagerActor));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await CoordinatedShutdown
            .Get(_actorSystem)
            .Run(CoordinatedShutdown.ClrExitReason.Instance);
    }

    public void Tell<T>(T message)
    {
        _calculationManager.Tell(message);
    }
}