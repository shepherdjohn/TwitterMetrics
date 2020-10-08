using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitterService.Entities;
using TwitterService.Shared;

namespace TwitterService.Services
{
    public class ChannelConsumerService : BackgroundService
    {
        public event EventHandler OnConsumerMessageReceived;

        private readonly ILogger<ChannelConsumerService> _logger;
        private ChannelReader<Message> _channelReader;

        public ChannelConsumerService(ILogger<ChannelConsumerService> logger, IMessageChannel channel)
        {
            _logger = logger;
            _channelReader = channel.Reader;

            TwitterLib.TrackerManager tracker = new TwitterLib.TrackerManager(10000);
            OnConsumerMessageReceived += tracker.OnNewStreamMessage;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int nullCounter = 0;

            try
            {

                while(!stoppingToken.IsCancellationRequested)
                {
                    var receivedMsg = await _channelReader.ReadAsync(stoppingToken);

                    if (receivedMsg == null)
                    {
                        nullCounter++;
                    }
                    else
                    {

                        MessageReceivedEventArgs args = new MessageReceivedEventArgs()
                        {
                            Message = receivedMsg
                        };

                        OnConsumerMessageReceived?.Invoke(this, args);
                    }
                   
                }
            }
           catch(OperationCanceledException)
            {
                _logger.LogInformation("Cancellation Token Exception Received");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An unhandled exception has occured {ex.ToString()}");
                
            }


        }
    }
}
