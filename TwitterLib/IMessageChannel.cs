using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TwitterService.Shared;

namespace TwitterService.Entities
{
    /**
    public interface IMessageChannel
    {

        ChannelReader<Message> Reader { get; }
        Task WriteMessageAsyn(Message item, CancellationToken cancelToken);
        void CompleteWriter(Exception ex = null);
        bool TryCompleteWriter(Exception ex = null);
    }
    **/
}
