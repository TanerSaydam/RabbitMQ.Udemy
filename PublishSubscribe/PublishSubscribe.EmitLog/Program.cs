using RabbitMQ.Client;
using System.Text;

namespace PublishSubscribe.EmitLog;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(" Press [enter] to send message");
        Console.ReadLine();

        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "logs", routingKey: string.Empty, basicProperties: null, body: body);

        Console.WriteLine($" [x] send {message}");

        Console.WriteLine(" Press [enter] to exit.");

        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(", ", args) : "Hello World!");
    }
}
