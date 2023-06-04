using Hocon.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

CreateAndConfigureApp(args).Run();

WebApplication CreateAndConfigureApp(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    var directory = AppDomain.CurrentDomain.BaseDirectory;
    Directory.SetCurrentDirectory(directory);

    builder.Host.ConfigureAppConfiguration((_, config) =>
    {
        config.Sources.Clear();
        config.SetBasePath(directory).AddHoconFile("settings.conf");
    });

    Console.WriteLine(builder.Configuration.GetDebugView());

    builder.Services.AddSingleton<IActorBridge, AkkaService>();
    builder.Services.AddHostedService<AkkaService>(sp => (AkkaService)sp.GetRequiredService<IActorBridge>());
    builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection(nameof(RabbitConfig)));
    builder.Services.AddRabbitClient();
    builder.Services.AddHealthChecks();
    builder.Services.AddControllers();

    var app = builder.Build();

    app.MapHealthChecks("/health");
    app.UseAuthorization();
    app.MapControllers();

    return app;
}