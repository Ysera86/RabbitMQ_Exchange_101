using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Exchange.Publisher
{
    public class TopicExchange
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

            string exchangeName = "logs-topic";
            channel.ExchangeDeclare(exchangeName, durable: true, type: ExchangeType.Topic); 

            #endregion

          
            // Kuyruk oluşturma Subscriberda

            #region 50 mesaj 

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                #region RouteKey oluşturma

                logNames log1 = (logNames)new Random().Next(1, 5);
                logNames log2 = (logNames)new Random().Next(1, 5);
                logNames log3 = (logNames)new Random().Next(1, 5);

                var routeKey = $"{log1}.{log2}.{log3}";  // RouteKey => Critical.Error.Warning gibi 

                #endregion


                logNames logName = (logNames)new Random().Next(1, 5);

                string message = $"log-type : {logName}-{x}  /  route : {log1}.{log2}.{log3}";

                var messageBody = Encoding.UTF8.GetBytes(message);



                channel.BasicPublish(exchangeName, routeKey, null, messageBody);

                Console.WriteLine($"Log gönderilmiştir : {message}");

            });

            #endregion


            Console.ReadLine();
        }
    }
}
