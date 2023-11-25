using RabbitMQ.Client;
using System.Text;

namespace WorkQueues.NewTask;

internal class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.HostName = "localhost";
        factory.Port = 5672;
        factory.UserName = "guest";
        factory.Password = "guest";

        //factory.Uri = new Uri("amqps://qswqssku:5V-JhWitvvVXTfbVfm_d3yUvMIIaS47P@rat.rmq2.cloudamqp.com/qswqssku");

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "task_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
           );

        string message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: "task_queue",
            basicProperties: properties,
            body: body);

        Console.WriteLine($" [*] Send {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
    static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}
