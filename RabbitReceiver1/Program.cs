using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

factory.ClientProvidedName = "Rabbit Receiver 1";

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "DemoExchange";

string routingKey = "demo-routing-key";

string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(5)).Wait();

    var body = args.Body.ToArray();

    // Get Message and Encode From UTF8
    string message = Encoding.UTF8.GetString(body);

    // Deserialize back into object


    Console.WriteLine($"Message Received: {message}");

    // Process message here...if message does not process do not acknowledge

    channel.BasicAck(args.DeliveryTag, false);

};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

// This is keeping app open while testing messages
Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();
