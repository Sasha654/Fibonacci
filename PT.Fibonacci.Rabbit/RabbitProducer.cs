using EasyNetQ;
using EasyNetQ.Topology;
using System.Threading.Tasks;

namespace PT.Fibonacci.Rabbit;

public sealed class RabbitProducer
{
    private readonly bool _mandatory = true;
    private readonly IAdvancedBus _bus;
    private readonly Exchange _exchange;
    private readonly string _routingKey;

    public RabbitProducer(IAdvancedBus bus, Exchange exchange, string routingKey)
    {
        _bus = bus;
        _exchange = exchange;
        _routingKey = routingKey;
    }

    public Task Produce<T>(T dto)
    {
        var message = new Message<T>(dto);

        return _bus.PublishAsync(
            _exchange,
            _routingKey,
            _mandatory,
            message);
    }
}