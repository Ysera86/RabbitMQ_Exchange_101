using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Exchange.Publisher
{
    public class DirectExchange
    {
        public enum logNames
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Info = 4,
        }

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

            #region Exchange Oluşturma

            string exchangeName = "logs-direct";
            channel.ExchangeDeclare(exchangeName, durable: true, type: ExchangeType.Direct); 

            #endregion

            #region Kuyruk oluşturma

            Enum.GetNames(typeof(logNames)).ToList().ForEach(logName =>
            {

                var routeKey = $"route-{logName}";

                var queueName = $"direct-queue-{logName}";
                channel.QueueDeclare(queueName, true, false, false);

                channel.QueueBind(queueName, exchangeName, routeKey);
            });

            #endregion


            #region 50 mesaj 

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                logNames logName = (logNames)new Random().Next(1, 5);

                string message = $"log-type : {logName}-{x}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{logName}";

                //channel.BasicPublish(string.Empty, "queue-name", null, messageBody);// artık def exchange değil ve kuyruk yok
                // RabbitMQ'dan "queue-name" isimli kuyruğu sildik. ve ilk çalıştırmada 50 mesajı yolladık ve Exchange tabında "logs-fanout" exchangeini gördük ancak henüz herhangi bir binding yok (ona bağlı bir subscriber yok), queue tabında da kuyruk yok. bu mesajlar havaya boşa gitti bekleyen herhangi bir subscriber yokıtu çünkü. Boşa gitmesin isteseydik  kuyruk oluştururduk. ilerde örnekte yapcaz.
                channel.BasicPublish(exchangeName, routeKey, null, messageBody);

                Console.WriteLine($"Log gönderilmiştir : {message}");

            });

            #endregion


            Console.ReadLine();
        }
    }
}
