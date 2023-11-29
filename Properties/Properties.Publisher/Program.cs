using RabbitMQ.Client;

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
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);
    }
}
