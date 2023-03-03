using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

factory.ClientProvidedName = "Rabbit MQ Demo";

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "DemoExchange";

string routingKey = "demo-routing-key";

string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for(int i = 0; i <60; i++)
{
    Console.WriteLine($"Sending Message {i}");

    // If data is an object Serialize as json


    // then encode it (UTF8)
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Message # {i}");

    // then send it
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

    Thread.Sleep(1000);
}

channel.Close();
connection.Close();
