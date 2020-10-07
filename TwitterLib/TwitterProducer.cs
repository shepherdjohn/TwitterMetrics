using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading.Channels;
using System.Threading;
using TwitterLib.Model;
using Microsoft.Extensions.Logging;

namespace TwitterLib
{
    public class TwitterProducer
    {
       
        private string accessToken = "AAAAAAAAAAAAAAAAAAAAACQOIAEAAAAAYLOKBJDsmVK922s058%2BIU4%2F4W2I%3DRLUKvlSBXDmgIdCbfSmdXa65JFGfPU6mdpZeX8ScrPfCVLEtQF";
        //private string uri = "https://api.twitter.com/2/tweets/sample/stream?expansions=attachments.media_keys";
        private string uri = "https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at,entities";
        private readonly ChannelWriter<Envelope> _writer;
        private readonly int _instanceId;


        public TwitterProducer(ChannelWriter<Envelope> writer, int instanceId)
        {
            _writer = writer;
            _instanceId = instanceId;
        }

        public async Task StartReaderAsync()
        {
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
                    await _writer.WriteAsync(new Envelope(json.Result));
                }
            }
            catch (OperationCanceledException ocException)
            {
                Console.WriteLine($"Operation was cancelled... {ocException.ToString()}");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


    }
}
