using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.SystemTextJson;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using System;

namespace PT.Fibonacci.Rabbit;

internal sealed class RabbitClientFactory : IRabbitClientFactory
{
    private const string QueueNamePrefix = "fibonacci_";
    private const string ExchangeName = "fibonacci.numbers";
    private readonly TimeSpan _heartBeat = TimeSpan.FromSeconds(10);
    private readonly IAdvancedBus _bus;

    public RabbitClientFactory(IOptions<RabbitConfig> rabbitConfig)
    {
        var config = rabbitConfig.Value;

        _bus = RabbitHutch
            .CreateBus(
                config.HostName,
                config.Port,
                config.VirtualHost,
                config.UserName,
                config.Password,
                _heartBeat,
                x =>
                    x.Register<ISerializer>(_ => new SystemTextJsonSerializer()))
            .Advanced;
    }

    public RabbitConsumer CreateConsumer(int id)
    {
        var queue = GetQueue(id);
        return new RabbitConsumer(_bus, queue);
    }

    public RabbitProducer CreateProducer(int id)
    {
        var routingKey = id.ToString();
        var queue = GetQueue(id);
        var exchange = GetExchange();
        _bus.Bind(exchange, queue, routingKey);
        return new RabbitProducer(_bus, exchange, routingKey);
    }

    private Queue GetQueue(int id)
    {
        var queueName = QueueNamePrefix + id;
        var queue = _bus.QueueDeclare(
            name: queueName,
            exclusive: false,
            durable: false,
            autoDelete: true);

        return queue;
    }

    private Exchange GetExchange()
    {
        var exchange = _bus.ExchangeDeclare(
            ExchangeName,
            ExchangeType.Direct,
            durable: false,
            autoDelete: true);

        return exchange;
    }
}