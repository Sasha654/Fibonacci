using EasyNetQ;
using EasyNetQ.Topology;
using System;
using System.Threading.Tasks;

namespace PT.Fibonacci.Rabbit;

public sealed class RabbitConsumer
{
    private readonly IAdvancedBus _bus;
    private readonly Queue _queue;

    public RabbitConsumer(IAdvancedBus bus, Queue queue)
    {
        _bus = bus;
        _queue = queue;
    }

    public IDisposable Consume<T>(Func<T, Task> callback)
    {
        var subscription = _bus.Consume<T>(
            _queue,
            (message, _) => callback(message.Body));

        return subscription;
    }
}