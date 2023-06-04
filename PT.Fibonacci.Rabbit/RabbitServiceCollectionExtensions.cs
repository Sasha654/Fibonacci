using Microsoft.Extensions.DependencyInjection;

namespace PT.Fibonacci.Rabbit;

/// <summary>
/// Методы расширения для настройки <see cref="IServiceCollection"/> для <see cref="IRabbitClientFactory"/>
/// </summary>
public static class RabbitServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет <see cref="IRabbitClientFactory"/> в <see cref="IServiceCollection"/>.
    /// </summary>
    public static IServiceCollection AddRabbitClient(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitClientFactory, RabbitClientFactory>();

        return services;
    }
}