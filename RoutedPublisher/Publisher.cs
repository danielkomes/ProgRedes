﻿using System;
using System.Text;
using Domain;
using LogServer;
using RabbitMQ.Client;

namespace RoutedPublisher
{
    public static class Publisher
    {
        private const string ExchangeName = "RoutedExchange";
        private const string RoutingKey = "LogServer";

        static void Main(string[] args)
        {

        }
        static IModel CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            DeclareExchange(channel);
            //Console.WriteLine("Enter messages to send or type exit to finish");
            //SendMessages(channel);
            return channel;
        }

        private static void DeclareExchange(IModel channel)
        {
            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        }

        //private static void SendMessages(IModel channel)
        //{
        //    string message = string.Empty;
        //    while (!message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        message = Console.ReadLine();
        //        if (!string.IsNullOrEmpty(message) || !message.Equals(ExitMessage, StringComparison.InvariantCultureIgnoreCase))
        //            PublishMessage(channel, message);
        //    }
        //}
        private static string EncodeLogEntry(LogEntry entry)
        {
            string date = entry.Date.ToString();
            string action = entry.Action.ToString();
            string clientName = entry.ClientName;
            string game = entry.AGame != null ? Logic.EncodeGame(entry.AGame) : "";

            string review = entry.AReview != null ? Logic.EncodeReview(entry.AReview) : "";
            string log = date + "@" + action + "@" + clientName + "@" + game + "@" + review; //To do: const string separator
            return log;
        }

        public static void PublishMessage(LogEntry entry)
        {
            string LogEntry = EncodeLogEntry(entry);
            IModel channel = CreateConnection();
            byte[] body = Encoding.UTF8.GetBytes(LogEntry);
            string routingKey = RoutingKey;

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}