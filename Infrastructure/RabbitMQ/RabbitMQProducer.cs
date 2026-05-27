using System.Text;
using System.Text.Json;
using PublisherEventAlert.Domain;
using RabbitMQ.Client;

namespace PublisherEventAlert.Infrastructure.RabbitMQ;

public class RabbitMQProducer
{
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly string _routingKey;

    public RabbitMQProducer(RabbitMQConnection connection, string exchange, string routingKey)
    {
        _channel = connection.Channel;
        _exchange = exchange;
        _routingKey = routingKey;
    }

    public void PublishMessage(AlertEvent @event)
    {
        var json = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.DeliveryMode = 2; // persistente
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        // mandatory:true => si ningún binding enruta el mensaje, RabbitMQ lo devuelve
        // (lo captura RabbitMQConnection.OnBasicReturn) en lugar de descartarlo silenciosamente.
        _channel.BasicPublish(
            exchange: _exchange,
            routingKey: _routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body);
    }
}
