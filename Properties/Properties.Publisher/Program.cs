using RabbitMQ.Client;
using System.Text;

namespace Properties.Publisher;

internal class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.HostName = "localhost";

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        var arguments = new Dictionary<string, object>();

        #region arguments
        arguments.Add("x-message-ttl", 5000); //Mesajların Kuyrukta kalabileceği mak süre (ms cinsinden)
        arguments.Add("x-expires", 6000); //Kuyruğun kullanılmadığında ne kadar süre içinde silineceği (ms)
        arguments.Add("x-max-length", 500); //kuyrukta tutulabilecek maksimum mesaj sayısı Bu limit aşıldığında kuruk başındaki eski mesajlar silinir veya başka bir kuyruğa yönlendirilir.
        arguments.Add("x-max-length-bytes", 104576); //Kuyruğun alabileceği maksimum ağırlığı byte cinsinden belirtir. (104576 => Maksimum 1MB)
        arguments.Add("x-dead-letter-exchange", "my.dead.letter.excahnge"); //Mesajın işlenmediği veya belirli bir kurala göre işlenmesi gerektiği durumlarda bu mesajların yönlendirileceği ölü harf değişimi (Dead Letter Exchange) belirtir
        arguments.Add("x-dead-letter-routing-key", "deadLetter"); //Ölü harf kuyruğuna mesaj gönderilirken kullanılacak yönlendirme anahtarını belirtir
        arguments.Add("x-max-priority", 10); //Kuyruğun desteklediği maksimum öncelik sayısını belirtir. Bu öncelikli kuyruk mekanizması kullanılarak mesajların işlenme sırasını kontrol etmekte kullanılır
        arguments.Add("x-queue-mode", "lazy"); //Kuyruğun çalışma modunu belirtir. Örneğin "lazy" modu mesajların diske yazılmasını sağlayarak bellek kullanımını azaltır. //default ve lazy parametrelerini alabiliyor. - Default mesajları bellekte tutar ve mesajların hızlı bir şekilde işlenmesini sağlar fakat yüksek bellek kullanımına neden olur.
        arguments.Add("x-queue-master-locator", ""); //Birden fazla kopası olan kuyrukları için ana kuyruk kopyasının hangi kriterlere göre seçileceğini belirtir.
        #endregion

        channel.QueueDeclare(
            queue: "hello",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);

        var properties = channel.CreateBasicProperties();

        #region basicProperties
        properties.Persistent = true; // Mesajın kalıcı olarak işaretlenmesi. True kalıcı false geçici
        properties.ContentType = "application/json"; // Mesajın içerik türünü ayarlar
        properties.ContentEncoding = "UTF-8"; // Mesajın içerik kodlamasını ayarlar
        properties.Priority = 5; // Mesajın önceliğini gösterir, daha yüksek öncelikli mesajlar önce işlenir. (0-9 arasında bir değer)
        properties.CorrelationId = Guid.NewGuid().ToString(); // İstek-yanıt işlemlerinde, bir isteğin hangi yanıtla ilişkili olduğunu belirlemek için kullanılır.
        properties.ReplyTo = "reply_queue"; // Yanıtın gönderileceği kuyruk ismini belirtir.
        properties.Expiration = "60000"; // Mesajın son kullanma süresi (milisaniye cinsinden)
        properties.MessageId = Guid.NewGuid().ToString(); // Mesajın benzersiz bir tanımlayıcısı.
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        // Mesajın oluşturulduğu zaman damgası.
        properties.Type = "command"; // Mesajın türünü tanımlar, örneğin bir komut veya event.
        properties.UserId = "user123"; // Mesajı gönderen kullanıcının kimliğini belirtir.
        properties.AppId = "app123"; // Mesajı gönderen uygulamanın kimliğini belirtir.
        #endregion

        string message = "{data: 'Hello World!'}";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                             routingKey: "test_queue",
                             basicProperties: properties,
                             body: body);

        Console.WriteLine(" [x] Sent {0}", message);
    }
}
