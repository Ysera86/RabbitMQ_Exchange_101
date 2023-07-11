using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ_Exchange.Publisher
{
    public class HeaderExchangeComplexType
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

            // Mesajlar kalıcı olsun
            properties.Persistent = true;

            Product product = new() { Id = 1, Name = "Kalem", Price = 110, Stock = 200 };
            var productJson = JsonSerializer.Serialize(product);

            channel.BasicPublish(exchangeName, string.Empty, properties, Encoding.UTF8.GetBytes(productJson));
            // routeKey empty çnk header exchange headerda tutuyor : properties

            Console.WriteLine("Mesaj gönderilmiştir.");

            #endregion


            Console.ReadLine();
        }
    }
}
