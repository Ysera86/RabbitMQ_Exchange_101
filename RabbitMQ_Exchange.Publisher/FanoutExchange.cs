using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Exchange.Publisher
{
    public class FanoutExchange
    {
        public void Run()
        {

            var factory = new ConnectionFactory();

            //factory.Uri = new Uri("http://localhost:15672/#/"); // AMQP URL 
            // connect to RabbitMQ
            factory.Port = 5672;
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("queue-name", true, false, false);
            // durable : kuyruklar memoryde olursa RabbitMQ restart olduğunda memorydeki kuyruklar gider durable false ise,gitsin  mi gitmesin m i burda hadi gitmesin, fiziksek kaydedilsin hadi
            // exclusive: false , subscriber bu kanal olmazsa da başka biyerden de erişebilsin farklı kanallardan 
            // autoDelete: true , son subscriber da down olsa kuyruk silinsin mi? yok gitmesin kuyruk

            #region 1 er mesaj

            //string message = "Hello, World!";

            //// mesajlar byte[] olarak gönderilir RabbitMQya
            //var messageBody = Encoding.UTF8.GetBytes(message);

            //channel.BasicPublish(string.Empty, "queue-name", null, messageBody);
            //// biz burada exchange kullanmadan direk kuyruğa yolluyoruz mesajı, arada exchange kullanmadan direk publisherdan kuyruğa gönderirsek bu işlemin adı default exchange  olarak geçer.
            //// default echange kullanıyorska buradaki routingkeyimize mutlaka kuyruk adımızı vermeliyiz > channel.QueueDeclare("queue-name", true, false, false);."queue-name" - ki buna göre gelen mesajı bu kuyruğa gönderebilsin. 
            //Console.WriteLine("Mesaj gönderilmiştir.");

            #endregion

            #region 50 mesaj 

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Message {x}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, "queue-name", null, messageBody);

                Console.WriteLine($"Mesaj gönderilmiştir : {message}");

            });

            #endregion


            Console.ReadLine();
        }
    }
}
