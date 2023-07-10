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

            // create exchange and not queue (leave to the subscriber)
            string exchangeName = "logs-fanout";
            channel.ExchangeDeclare(exchangeName, durable:true,type: ExchangeType.Fanout);
       
            #region 50 mesaj 

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"log {x}";

                var messageBody = Encoding.UTF8.GetBytes(message);


                //channel.BasicPublish(string.Empty, "queue-name", null, messageBody);// artık def exchange değil ve kuyruk yok
                // RabbitMQ'dan "queue-name" isimli kuyruğu sildik. ve ilk çalıştırmada 50 mesajı yolladık ve Exchange tabında "logs-fanout" exchangeini gördük ancak henüz herhangi bir binding yok (ona bağlı bir subscriber yok), queue tabında da kuyruk yok. bu mesajlar havaya boşa gitti bekleyen herhangi bir subscriber yokıtu çünkü
                channel.BasicPublish(exchangeName, "", null, messageBody);

                Console.WriteLine($"Mesaj gönderilmiştir : {message}");

            });

            #endregion


            Console.ReadLine();
        }
    }
}
