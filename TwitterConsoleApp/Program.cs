using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using TwitterLib;
using TwitterLib.Model;

namespace TwitterConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var channel = Channel.CreateBounded<Envelope>(1000);

            var stats = new TrackerManager(10000);
            //var testDataBuilder = new TestDataBuilder() { MsgToWrite = 3000 };
           
            Console.WriteLine("Starting Producer.....");
            var producer = new TwitterProducer(channel, 1);
            var producerTask = producer.StartReaderAsync();
            Console.WriteLine("Producer Started");

            Console.WriteLine("Starting Consumer.....");
            var consumer = new TwitterConsumer(channel, 1);
            consumer.StreamDataReceivedEvent += stats.OnNewStreamMessage;
            //consumer.StreamDataReceivedEvent += testDataBuilder.OnNewStreamMessage;
            var consumerTask = consumer.BeginConsumerAsync();
            Console.WriteLine("Consumer Started");

            Console.WriteLine("Waiting Tasks");
            Task.WaitAll(producerTask, consumerTask);
            Console.WriteLine("Wait all Complete");

        }
    }
}
