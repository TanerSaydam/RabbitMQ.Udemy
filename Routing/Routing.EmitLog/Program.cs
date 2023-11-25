using RabbitMQ.Client;
using System.Text;

namespace Routing.EmitLog;

internal class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

        var severity = args.Length > 0 ? args[0] : "info";
        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "direct_logs", 
            routingKey: severity, 
            basicProperties: null, body: body);

        Console.WriteLine($" [x] send {severity} : {message}");

        Console.WriteLine(" Press [enter] to exit.");

        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 1) ? string.Join(", ", args.Skip(1).ToArray()) : "Hello World!");
    }
}

