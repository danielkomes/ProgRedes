using System;
using System.Text;
using LogServer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class SubscriberProgram
    {
        private const string ExchangeName = "RoutedExchange";
        private const string RoutingKey = "LogServer";

        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            DeclareExchange(channel);
            string queueName = channel.QueueDeclare().QueueName;
            DeclareQueue(channel, queueName);
            ReceiveMessages(channel, queueName);
            Console.WriteLine("This will get even length messages");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void DeclareExchange(IModel channel)
        {
            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        }

        private static void DeclareQueue(IModel channel, string queueName)
        {
            channel.QueueBind(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: RoutingKey);
        }

        private static void ReceiveMessages(IModel channel, string queueName)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message : " + message);
                Logs.Add(LogEntry.DecodeLogEntry(message));
            };

            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }
    }
}
