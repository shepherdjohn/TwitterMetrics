using TwitterLib.Model;

namespace TwitterLib.Tracker
{
    public interface ITracker
    {
        string TrackerName { get; }
        int MsgCount { get; }
        void OnNewMessage(TwitterStreamModel msg);
        object GetResults();
       

    }

    public interface IEmojiTracker : ITracker
    {

    }

    public interface IUrlTracker : IEntitiesTracker
    {
        int PhotoCount { get; }
        double PhotoPct { get; }
        int TweetsWithPhotos { get; }
        int TweetsWithUrl { get; }
      
    }

    public interface IEntitiesTracker : ITracker
    {
        int QualifiedMsgCount { get; }
        
     }

    public interface IMatchTracker
    {
        string MatchString { get; }
        object GetTotalMatchCount();
    }

    public interface ICountTracker : IMatchTracker, ITracker
    {
       
    }

    public interface IListTracker : IMatchTracker, ITracker
    {
       
        
    }

    public interface IRateTracker :ITracker
    {
        int Interval { get; }
    }

}
