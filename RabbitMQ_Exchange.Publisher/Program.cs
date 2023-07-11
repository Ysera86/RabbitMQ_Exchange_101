using RabbitMQ.Client;
using RabbitMQ_Exchange.Publisher;
using System.Text;


//FanoutExchange fanoutExchange = new FanoutExchange();   
//fanoutExchange.Run();

//DirectExchange directExchange = new DirectExchange();
//directExchange.Run();

//TopicExchange topicExchange = new TopicExchange();
//topicExchange.Run();

//HeaderExchange headerExchange = new HeaderExchange();
//headerExchange.Run();

HeaderExchangeComplexType headerExchangeComplexType = new HeaderExchangeComplexType();
headerExchangeComplexType.Run();
