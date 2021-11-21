using LogHandler;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RoutedPublisher
{
    public static class Publisher
    {
        private const string ExchangeName = "RoutedExchange";
        private const string RoutingKey = "LogServer";

        static void Main()
        {
            //ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
            //using IConnection connection = factory.CreateConnection();
            //using IModel channel = connection.CreateModel();
            //DeclareExchange(channel);
            //Console.WriteLine("Enter messages to send or type exit to finish");
            //SendMessages(channel);
        }

        private static void DeclareExchange(IModel channel)
        {
            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        }

        private static void SendMessages(IModel channel)
        {
            string message = string.Empty;
            while (true)
            {
                message = Console.ReadLine();
                //if (!string.IsNullOrEmpty(message) || !message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
                //PublishMessage(channel, message);
            }
        }

        public static void PublishMessage(LogEntry entry)
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            DeclareExchange(channel);

            string entryString = entry.EncodeLogEntry();
            byte[] body = Encoding.UTF8.GetBytes(entryString);
            string routingKey = RoutingKey;

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}