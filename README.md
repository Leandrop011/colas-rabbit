# PublisherEventAlert — Sistema de Mensajeria Asincrona con RabbitMQ

Aplicacion CLI desarrollada en C# .NET 8 que actua como productor de mensajes hacia un broker RabbitMQ desplegado con Docker.

---

## Tecnologias

- C# .NET 8
- RabbitMQ 3.12 (AMQP 0-9-1)
- Docker

---

## Estructura del proyecto

```
PublisherEventAlert/
├── Domain/
│   └── AlertEvent.cs
├── Infrastructure/
│   └── RabbitMQ/
│       ├── RabbitMQConnection.cs
│       └── RabbitMQProducer.cs
└── Program.cs
```

---

## Requisitos previos

- .NET 8 SDK instalado
- Docker Desktop corriendo
- RabbitMQ levantado con el siguiente comando:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -v rabbitmq_data:/var/lib/rabbitmq rabbitmq:3.12-management
```

---

## Configuracion de RabbitMQ

Antes de ejecutar la aplicacion, asegurarse de tener configurado lo siguiente en el panel de administracion (localhost:15672):

**Virtual host:** dev

**Exchanges:**
- dev.direct (type: direct, durable)
- dev.fanout (type: fanout, durable)
- dev.topic (type: topic, durable)

**Colas:**
- dev.inventario
- dev.logistica
- dev.notifications

**Bindings:**
- dev.direct -> dev.inventario (routing key: inventario)
- dev.direct -> dev.logistica (routing key: logistica)
- dev.direct -> dev.notifications (routing key: notificacion.cliente)
- dev.fanout -> dev.notifications (sin routing key)
- dev.topic -> dev.notifications (routing key: log.*)

---

## Ejecucion

```bash
cd PublisherEventAlert
dotnet run
```

---

## Uso

Al iniciar la aplicacion se muestra el siguiente menu:

```
===== Publisher Event Alert =====
1) Publicar a dev.direct
2) Publicar a dev.fanout
3) Publicar a dev.topic
4) Salir
Seleccione una opcion:
```

- Opcion 1: Publica a una cola especifica segun routing key (inventario, logistica, notificacion.cliente)
- Opcion 2: Publica a todas las colas vinculadas al exchange fanout
- Opcion 3: Publica segun patron de routing key (log.error, log.info, log.warning)

---

## Autores

- Leandro Pozo
- Miguel Punin
- Erick Quinchiguango

Universidad de las Americas — UDLA
Materia: Diseno y Arquitectura de Software
Mayo 2026
