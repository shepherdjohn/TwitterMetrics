using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwitterService.Shared;

namespace TwitterService.Entities
{
    public interface IMessageChannel
    {

        ChannelReader<Message> Reader { get; }
        Task WriteMessageAsyn(Message item, CancellationToken cancelToken);
        void CompleteWriter(Exception ex = null);
        bool TryCompleteWriter(Exception ex = null);
    }

    public class MessageChannel: IMessageChannel
    {
        private ChannelReader<Message> _reader;
        private Channel<Message> _channel;
        private readonly ILogger<MessageChannel> _logger;


        public MessageChannel(ILogger<MessageChannel> logger)
        {
            _logger = logger;

            _channel = Channel.CreateBounded< Message>(new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = true,
                SingleReader = true
            });

            _reader = _channel.Reader;
        }

        public ChannelReader<Message> Reader { get { return _reader; } }

        public int MaxMessagesInChannel { get; private set; } = 1000;

        public void CompleteWriter(Exception ex = null) => _channel.Writer.Complete(ex);

        public bool TryCompleteWriter(Exception ex = null) => _channel.Writer.TryComplete(ex);
       
        public async Task WriteMessageAsyn(Message item, CancellationToken cancelToken)
        {
            try
            {
                await _channel.Writer.WriteAsync(item, cancelToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
           

        }

      
    }
}
