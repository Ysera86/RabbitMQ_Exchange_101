using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


/*
 Önemli :  Diyelim 2 subscriber var, kaç mesaj yollanabilir belirleyeiliyoruz.
ya subs1 e 1, subs2ye 1 ya da 5er desek subs1e 5 subs2ye 5. 
Eğer mesjaalrın işlenmesi uzun sürüyosa ve 7 mesaj varsa 5er 5er yollarken, subs2 2 taneyi işlediğinde subs1 hala 3 tane daha işlemeli, subs2 onun tirmesini bekler. Bu nedenle kaçar mesay alabileceğini/ yollanabileceğini seçerken buna dikkat etmek gerek.
Mesaj işlemek uzun sürüyosa azar azar, kısa sürüyosa çok sayıda yollanması uygun olabilir!
 
 */

var factory = new ConnectionFactory();
factory.Port = 5672;
factory.HostName = "localhost";
factory.UserName = "guest";
factory.Password = "guest";

using var connection = factory.CreateConnection();
var channel = connection.CreateModel();

//channel.ExchangeDeclare(exchangeName, durable: true, type: ExchangeType.Fanout);
// İstesek exchange i burda da oluştururduk, nasılsa producer oluşturdu hata almazdık. Kuyruk oluşturmadaki gibi aynı konfigurasyonlarla oluşturduktan snr sorun yok.


//channel.QueueDeclare("queue-name", true, false, false);
// 1 - bu satırı silersek ve daha önce publisher bu isimde kuyruk oluşturmamış olursak hata alırız.
// 2 - bu satırı bırakırız ve publisher yine daha öncesinde bu isimde kuyruk oluşturmamış olursa subscriber oluşturur.
// publisher kesin oluşturduysa, eminsek silebiliriz.
// zaten varsa kuyruk burada da olması hata vermez. Sadece aynı isimde kuyruk oluşturuyosak tamamen aynı parametrelerle oluşturduğumuzdan  emin olmalıyız.


// farklı kuyruklar olmalı, consumer instance arttıkça kuyruk ismi farklı olmalı. random oluşturalım.
var randomQueueName = channel.QueueDeclare().QueueName;

// channel.QueueDeclare  : satır :27 dersem, subsc işini bitirse dahi ilgili kuyruk silinmez ,

string exchangeName = "logs-fanout"; // producer böyle oluşturdu
// subsc işini bitirince ilgili kuyruk silinsin , uygulama her ayağa kalktığında oluşan kuyruk, uygulama her kapandığında silinecek, çnk QueueDeclare etmedik! Bind ettik!
channel.QueueBind(randomQueueName, exchangeName, "", null);



channel.BasicQos(0, 1, false); // her subscribera 1er mesaj yolla
/*
 2 tane subscriber instance çalıştırmak için cli ile bsubscriber proje dizinine gidip clidan çalıştırıcam, 2 ayrı cli mesela 
şuan publisher 50 mesaj yolladı, kuyrkta bekliyorlar, aşırı hızlı alıyor mesajları tek taraf bu nedenle 1,5 sn bekletelim : Thread.Sleep(1500);

cd C:\Users\merve\source\repos\RabbitMQ_Exchange_101\RabbitMQ_Exchange.Subscriber
dotnet run
 

her ayağa kalkan subscriber instance ı rabbitmqde exchange e bind olur. exchange tabında binding görülebilir.
 */

#region Açıklaması 
//channel.BasicQos(0, 6, false); // Kuyruktaki her subscribera 6şar mesaj yolla
//channel.BasicQos(0, 6, true);  // Bütün subscriberlara yolladığın mesajların toplamı 6 olsun, mesela 2 taneyse 3 e 3 gibi aralarında böle, 6 tanyse 1erli, 3 taneyse 2şerli gibi. 
#endregion

var consumer = new EventingBasicConsumer(channel);
//channel.BasicConsume("queue-name", true, consumer);
/* autoAck : true > RabbitMQ subscribera bi mesaj gönderdiğinde, bu mesaj doğru da işlense yanlış da işlense RabbitMQ bu mesajı kuyruktan siler
 autoAck : false ise sen bunu direk silme, mesaj doğru işlenirse ben sana silmen için haber vericem demiş oluyoruz. > gerçek dünyada */


//channel.BasicConsume("queue-name", false, consumer); // mesajları hemen silme ben haber vericem doğr uişlendiğinde o zaman sil
channel.BasicConsume(randomQueueName, false, consumer); // artık oluşan kuyruk ne ise onu dinle. 

Console.WriteLine($" Loglar dinleniyor...");

consumer.Received += (sender, args) =>
{
    var message = Encoding.UTF8.GetString(args.Body.ToArray());

    Thread.Sleep(1500);
    Console.WriteLine($"Gelen mesaj : {message}");

    channel.BasicAck(args.DeliveryTag, false); // hata varsa bunu sölemicez, RabbitMQ da işlendi mesajı aımayan mesajları 
    // true : memorydeki her işlenen ama rabbitMqya gönderilmeyen mesajları sil .
    // false :  mesajları teker teker işlediğim için teker teker sil diyorum

};

Console.ReadLine();