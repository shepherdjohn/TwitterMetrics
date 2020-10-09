using System;
using TwitterLib.Model;

namespace TwitterLib
{
    public class Util
    {
     
        public class TweetReceivedEventArgs : EventArgs
        {
            public TwitterStreamModel TwitterStreamModel { get; set; }
        }

    }
}
