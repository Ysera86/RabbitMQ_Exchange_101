using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ_Exchange.Subscriber
{
    public class HeaderExchangeComplexType
    {
        public void Run()
        {

            var factory = new ConnectionFactory();
            factory.Port = 5672;
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";


            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            string exchangeName = "header-exchange";

            var queueName = channel.QueueDeclare().QueueName;


            #region Kuyruk oluşturma :  QueueBind : Subscriber düşünce kuyruk da düşsün

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all"); // üstteki diğer tüm key-valuelar publisherınkilerle uymalı.

            channel.QueueBind(queueName, exchangeName, string.Empty, headers);


            #endregion


            channel.BasicQos(0, 1, false);

            /*
             *
             * 
            2 tane subscriber instance çalıştırmak için cli ile subscriber proje dizinine gidip clidan çalıştırıcam, 2 ayrı cli mesela 
           şuan publisher 50 mesaj yolladı, kuyrkta bekliyorlar, aşırı hızlı alıyor mesajları tek taraf bu nedenle 1,5 sn bekletelim : Thread.Sleep(1500);

           cd C:\Users\merve\source\repos\RabbitMQ_Exchange_101\RabbitMQ_Exchange.Subscriber
           dotnet run


           her ayağa kalkan subscriber instance ı rabbitmqde exchange e bind olur. exchange tabında binding görülebilir.
            */


            var consumer = new EventingBasicConsumer(channel);

            
            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine($" Loglar dinleniyor...");

            consumer.Received += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray()); // artık product json

                Product product = JsonSerializer.Deserialize<Product>(message);

                Thread.Sleep(1500);
                Console.WriteLine($"Gelen mesaj => Id :{product.Id} - Name :{product.Name} - Price :{product.Price} - Stock :{product.Stock}");

                channel.BasicAck(args.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
