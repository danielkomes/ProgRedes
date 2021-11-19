using System;
using System.Text;
using RabbitMQ.Client;

namespace RoutedPublisher
{
    class RoutedPublisherProgram
    {
        private const string ExchangeName = "n6aRoutedExchange";
        private const string ExitMessage = "exit";
        private const string EvenRoutingKey = "even";
        private const string UnEvenRoutingKey = "unEven";

        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            DeclareExchange(channel);
            Console.WriteLine("Enter messages to send or type exit to finish");
            SendMessages(channel);
        }

        private static void DeclareExchange(IModel channel)
        {
            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        }

        private static void SendMessages(IModel channel)
        {
            string message = string.Empty;
            while (!message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
            {
                message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message) || !message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
                    PublishMessage(channel, message);
            }
        }

        private static void PublishMessage(IModel channel, string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            string routingKey = message.Length % 2 == 0 ? EvenRoutingKey : UnEvenRoutingKey;

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}
