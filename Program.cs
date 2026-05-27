using PublisherEventAlert.Domain;
using PublisherEventAlert.Infrastructure.RabbitMQ;

Console.OutputEncoding = System.Text.Encoding.UTF8;

try
{
    using var connection = new RabbitMQConnection();
    Console.WriteLine("Conexión a RabbitMQ establecida (localhost:5672, vhost: dev).");

    var exit = false;
    while (!exit)
    {
        PrintMenu();
        var option = Console.ReadLine()?.Trim();

        switch (option)
        {
            case "1":
                PublishDirect(connection);
                break;
            case "2":
                PublishFanout(connection);
                break;
            case "3":
                PublishTopic(connection);
                break;
            case "4":
                exit = true;
                Console.WriteLine("Saliendo...");
                break;
            default:
                Console.WriteLine("Opción no válida. Intente nuevamente.");
                break;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("===== Publisher Event Alert =====");
    Console.WriteLine("1) Publicar a dev.direct");
    Console.WriteLine("2) Publicar a dev.fanout");
    Console.WriteLine("3) Publicar a dev.topic");
    Console.WriteLine("4) Salir");
    Console.Write("Seleccione una opción: ");
}

static void PublishDirect(RabbitMQConnection connection)
{
    Console.WriteLine();
    Console.WriteLine("--- Publicar a dev.direct ---");
    Console.WriteLine("Routing keys disponibles: inventario | logistica | notificacion.cliente");
    Console.Write("Ingrese routing key: ");
    var routingKey = Console.ReadLine()?.Trim() ?? string.Empty;

    var validKeys = new[] { "inventario", "logistica", "notificacion.cliente" };
    if (!validKeys.Contains(routingKey))
    {
        Console.WriteLine("Routing key inválida. Operación cancelada.");
        return;
    }

    var @event = BuildAlertEvent();
    if (@event == null) return;

    var producer = new RabbitMQProducer(connection, "dev.direct", routingKey);
    producer.PublishMessage(@event);

    Console.WriteLine($"✔ Mensaje publicado en exchange 'dev.direct' con routing key '{routingKey}'.");
}

static void PublishFanout(RabbitMQConnection connection)
{
    Console.WriteLine();
    Console.WriteLine("--- Publicar a dev.fanout ---");

    var @event = BuildAlertEvent();
    if (@event == null) return;

    var producer = new RabbitMQProducer(connection, "dev.fanout", string.Empty);
    producer.PublishMessage(@event);

    Console.WriteLine("✔ Mensaje publicado en exchange 'dev.fanout' (broadcast).");
}

static void PublishTopic(RabbitMQConnection connection)
{
    Console.WriteLine();
    Console.WriteLine("--- Publicar a dev.topic ---");
    Console.WriteLine("Binding existente: dev.topic -> dev.notifications con patrón 'notificacion.*'");
    Console.WriteLine("Ejemplos válidos: notificacion.cliente | notificacion.sistema | notificacion.alerta");
    Console.WriteLine("(Solo claves que coincidan con 'notificacion.*' llegan a dev.notifications.)");
    Console.Write("Ingrese routing key: ");
    var routingKey = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(routingKey))
    {
        Console.WriteLine("Routing key no puede estar vacía. Operación cancelada.");
        return;
    }

    var @event = BuildAlertEvent();
    if (@event == null) return;

    var producer = new RabbitMQProducer(connection, "dev.topic", routingKey);
    producer.PublishMessage(@event);

    Console.WriteLine($"✔ Mensaje publicado en exchange 'dev.topic' con routing key '{routingKey}'.");
}

static AlertEvent? BuildAlertEvent()
{
    Console.Write("Contenido del mensaje: ");
    var message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
    {
        Console.WriteLine("El mensaje no puede estar vacío. Operación cancelada.");
        return null;
    }

    Console.Write("Severidad (Info | Warning | Alerta): ");
    var severity = Console.ReadLine()?.Trim();

    var validSeverities = new[] { "Info", "Warning", "Alerta" };
    if (string.IsNullOrWhiteSpace(severity) ||
        !validSeverities.Contains(severity, StringComparer.OrdinalIgnoreCase))
    {
        Console.WriteLine("Severidad inválida. Operación cancelada.");
        return null;
    }

    return new AlertEvent(message, severity);
}
