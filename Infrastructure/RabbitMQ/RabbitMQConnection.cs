using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PublisherEventAlert.Infrastructure.RabbitMQ;

public class RabbitMQConnection : IDisposable
{
    // Topología REAL existente en RabbitMQ (no se declara ni se modifica desde aquí).
    public const string HostName = "localhost";
    public const int Port = 5672;
    public const string VirtualHost = "dev";

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public IModel Channel => _channel;

    public RabbitMQConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = HostName,
            Port = Port,
            VirtualHost = VirtualHost,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Si un mensaje se publica con mandatory:true y ningún binding lo enruta,
        // RabbitMQ lo devuelve aquí en vez de descartarlo en silencio.
        _channel.BasicReturn += OnBasicReturn;
    }

    private static void OnBasicReturn(object? sender, BasicReturnEventArgs args)
    {
        Console.WriteLine(
            $"⚠ Mensaje NO enrutado: exchange '{args.Exchange}', routing key '{args.RoutingKey}' " +
            $"({args.ReplyCode} {args.ReplyText}). Ningún binding coincide; el mensaje no llegó a ninguna cola.");
    }

    public void Dispose()
    {
        _channel.BasicReturn -= OnBasicReturn;

        if (_channel.IsOpen)
            _channel.Close();

        if (_connection.IsOpen)
            _connection.Close();

        _channel.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
