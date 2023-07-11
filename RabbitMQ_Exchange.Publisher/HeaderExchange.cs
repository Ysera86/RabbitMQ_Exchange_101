using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Exchange.Publisher
{
    public class HeaderExchange
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

            string exchangeName = "header-exchange";
            channel.ExchangeDeclare(exchangeName, durable: true, type: ExchangeType.Headers);

            #endregion

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            // kuyruk consumerda

            #region Message 

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;

            channel.BasicPublish(exchangeName, string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));
            // routeKey empty çnk header exchange headerda tutuyor : properties

            Console.WriteLine("Mesaj gönderilmiştir.");

            #endregion


            Console.ReadLine();
        }
    }
}
