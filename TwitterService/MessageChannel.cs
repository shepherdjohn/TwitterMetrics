using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwitterService.Shared;

namespace TwitterService.Entities
{
    /// <summary>
    /// This is a wrapper implimentation of Channel<Message>
    /// </summary>
    public class MessageChannel : IMessageChannel
    {
        private ChannelReader<Message> _reader;
        private Channel<Message> _channel;
        private readonly ILogger<MessageChannel> _logger;


        public MessageChannel(ILogger<MessageChannel> logger)
        {
            _logger = logger;

            _channel = Channel.CreateBounded<Message>(new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = true,
                SingleReader = true
            });

            _reader = _channel.Reader;
        }

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

        public ChannelReader<Message> Reader { get { return _reader; } }

        public int MaxMessagesInChannel { get; private set; } = 1000;

    }
}
