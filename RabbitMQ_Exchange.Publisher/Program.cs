using RabbitMQ.Client;
using RabbitMQ_Exchange.Publisher;
using System.Text;


FanoutExchange fanoutExchange = new FanoutExchange();   
fanoutExchange.Run();