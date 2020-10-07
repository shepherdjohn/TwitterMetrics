using System;
namespace TwitterService.Shared
{
    public interface IMessage
    {
        string Body { get; set; }
    }

    public class Message : IMessage
    {
        public string Body { get; set; }
    }

}
