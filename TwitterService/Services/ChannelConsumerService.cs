using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitterService.Entities;
using TwitterService.Shared;
using TwitterLib;

namespace TwitterService.Services
{
    public class ChannelConsumerService : BackgroundService
    {
        public event EventHandler OnConsumerMessageReceived;

        private readonly ILogger<ChannelConsumerService> _logger;
        private readonly ITrackerManager _trackerManager;
        private ChannelReader<Message> _channelReader;


        public ChannelConsumerService(ILogger<ChannelConsumerService> logger,ITrackerManager trackerManager, IMessageChannel channel)
        {
            _logger = logger;
            _trackerManager = trackerManager;
            _channelReader = channel.Reader;
           
            OnConsumerMessageReceived += _trackerManager.OnNewStreamMessage;
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
            
            try
            {
                while(!stoppingToken.IsCancellationRequested)
                {
                    var receivedMsg = await _channelReader.ReadAsync(stoppingToken);
                    if (!string.IsNullOrEmpty(receivedMsg.Body))
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
