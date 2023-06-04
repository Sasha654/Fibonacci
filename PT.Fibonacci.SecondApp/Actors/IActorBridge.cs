namespace PT.Fibonacci.SecondApp.Actors;

/// <summary>
/// Представляет интерфейс взаимодействия с системой акторов
/// </summary>
public interface IActorBridge
{
    /// <summary>
    /// Отправляет запрос системе акторов
    /// </summary>
    /// <typeparam name="T">Тип сообщения-запроса</typeparam>
    /// <param name="message">Сообщение-запрос</param>
    void Tell<T>(T message);
}