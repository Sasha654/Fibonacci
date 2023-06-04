namespace PT.Fibonacci.Rabbit;

public sealed class RabbitConfig
{
    public string HostName { get; set; } = "localhost";

    public ushort Port { get; set; } = 5672;

    public string VirtualHost { get; set; } = "/";

    public string UserName { get; set; } = "guest";

    public string Password { get; set; } = "guest";
}