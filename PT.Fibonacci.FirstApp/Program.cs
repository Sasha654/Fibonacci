using Hocon.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

try
{
    await using var serviceProvider = BuildServiceProvider();
    var config = serviceProvider.GetRequiredService<IOptions<FirstAppConfig>>().Value;
    var taskList = new List<Task>();
    using var cts = new CancellationTokenSource();

    for (var i = 1; i <= config.ParallelCalcCount; i++)
    {
        var service = serviceProvider.GetRequiredService<InitiatorService>();
        taskList.Add(service.Run(config.CalcRequestUri, i, cts.Token));
    }

    Console.WriteLine("Press 'Enter' to stop program");
    Console.ReadLine();
    cts.Cancel();
    await Task.WhenAll(taskList);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadLine();


ServiceProvider BuildServiceProvider()
{
    var config = new ConfigurationBuilder()
        .AddHoconFile("Settings.conf")
        .Build();

    Console.WriteLine(config.GetDebugView());

    var serviceProvider = new ServiceCollection()
        .AddLogging(b =>
        {
            b.AddConfiguration(config.GetSection("logging"));
            b.AddConsole();
        })
        .Configure<RabbitConfig>(config.GetSection(nameof(RabbitConfig)))
        .Configure<FirstAppConfig>(config.GetSection(nameof(FirstAppConfig)))
        .AddTransient<InitiatorService>()
        .AddHttpClient()
        .AddRabbitClient()
        .BuildServiceProvider();

    return serviceProvider;
}