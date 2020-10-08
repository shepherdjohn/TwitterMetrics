using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitterService.Entities;
using TwitterService.Shared;

namespace TwitterService.Services
{
    public class ChannelProducerService : BackgroundService

    {
        private readonly ILogger<ChannelProducerService> _logger;
        private readonly IMessageChannel _messageChannel;

        public ChannelProducerService(ILogger<ChannelProducerService> logger, IMessageChannel _messageChannel)
        {
            this._logger = logger;
            this._messageChannel = _messageChannel;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);

        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            string accessToken = "AAAAAAAAAAAAAAAAAAAAACQOIAEAAAAAYLOKBJDsmVK922s058%2BIU4%2F4W2I%3DRLUKvlSBXDmgIdCbfSmdXa65JFGfPU6mdpZeX8ScrPfCVLEtQF";
            string uri = "https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at,entities";
      
            try
            {

                var httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(0, 0, 10);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var requestUri = new HttpRequestMessage(HttpMethod.Get, uri);

                var streamReader = new StreamReader(await httpClient.GetStreamAsync(uri));

                while (!streamReader.EndOfStream)
                {
                    var json = streamReader.ReadLineAsync();
                    await _messageChannel.WriteMessageAsyn(new Message() { Body = json.Result }, stoppingToken);
                }
            }
            catch(TimeoutException)
            {
                _logger.LogWarning("Received Timeout from HTTP Client");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Cancellation Token Exception Received");
            }

            catch (Exception ex)
            {
                _logger.LogCritical($"An unhandled exception has occured {ex.ToString()}");
                _messageChannel.CompleteWriter(ex);
            }
            finally
            {
                _messageChannel.TryCompleteWriter();
            }
            
        }
    }
}
