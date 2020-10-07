using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitterLib.Model;
using static TwitterLib.Util;

namespace TwitterLib
{
    
    public class TwitterConsumer
    {
        public event EventHandler StreamDataReceivedEvent;
        private readonly ChannelReader<Envelope> _reader;
        private readonly int _instanceId;
       
        public TwitterConsumer(ChannelReader<Envelope> channel, int instanceId)
        {
            _reader = channel;
            _instanceId = instanceId;
        }

        public async Task BeginConsumerAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                while (await _reader.WaitToReadAsync())
                {
                    var envelope = await _reader.ReadAsync();
                    TwitterStreamModel model = JsonConvert.DeserializeObject<TwitterStreamModel>(envelope.Payload);
                    
                    if (StreamDataReceivedEvent!= null && model!=null)
                    {
                        StreamDataReceivedEvent(this, new TweetReceivedEventArgs() { TwitterStreamModel = model });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
          

        }


    }
}
