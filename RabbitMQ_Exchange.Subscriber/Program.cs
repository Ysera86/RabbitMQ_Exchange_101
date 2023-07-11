using RabbitMQ_Exchange.Subscriber;


//FanoutExchange fanoutExchange = new FanoutExchange();
//fanoutExchange.Run();

//DirectExchange directExchange = new DirectExchange();
//directExchange.Run();

TopicExchange topicExchange = new TopicExchange();
topicExchange.Run();
