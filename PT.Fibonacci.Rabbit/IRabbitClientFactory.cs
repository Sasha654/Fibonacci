namespace PT.Fibonacci.Rabbit;

/// <summary>
/// Фабрика создания клиентов для работы с RabbitMQ
/// </summary>
public interface IRabbitClientFactory
{
    /// <summary>
    /// Создает потребителя сообщений
    /// </summary>
    /// <param name="id">Идентификатор</param>
    RabbitConsumer CreateConsumer(int id);

    /// <summary>
    /// Создает производителя сообщений 
    /// </summary>
    /// <param name="id">Идентификатор</param>
    RabbitProducer CreateProducer(int id);
}