using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitterLib;
using TwitterLib.Model;

namespace TwitterService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
     
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
          
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var channel = Channel.CreateBounded<Envelope>(1000);

            var stats = new TrackerManager(10000);

            var producer = new TwitterProducer(channel, 1);
            var producerTask = producer.StartReaderAsync();
            var consumer = new TwitterConsumer(channel, 1);
            consumer.StreamDataReceivedEvent += stats.OnNewStreamMessage;
            var consumerTask = consumer.BeginConsumerAsync();

            Task.WaitAll(producerTask, consumerTask);

           
            return base.StartAsync(cancellationToken);
        }

        private Task[] StartConsumers(Channel<Envelope> channel, int consumersCount, CancellationToken cancellationToken)
        {
            var consumerTasks = Enumerable.Range(1, consumersCount)
                .Select(i => new TwitterConsumer(channel.Reader, i).BeginConsumerAsync(cancellationToken))
                .ToArray();
            return consumerTasks;
        }

       

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

      
        

    }
}
