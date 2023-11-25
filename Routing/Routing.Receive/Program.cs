using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Routing.Receive;

internal class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

        var queueName = channel.QueueDeclare().QueueName;

        var severity = args.Length > 0 ? args[0] : "info";

        channel.QueueBind(
            queue: queueName,
            exchange: "direct_logs",
            routingKey: severity);

        Console.WriteLine(" [*] Waiting for logs");

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] {severity} : {message} log");
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");

        Console.ReadLine();
    }
}
