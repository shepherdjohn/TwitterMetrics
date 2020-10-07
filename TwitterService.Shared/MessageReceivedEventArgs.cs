using System;


namespace TwitterService.Shared
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; set; }

    }
}
