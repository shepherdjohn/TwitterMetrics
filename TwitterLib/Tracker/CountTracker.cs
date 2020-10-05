using System;
using System.Text;
using System.Text.RegularExpressions;
using TwitterLib.Model;

namespace TwitterLib.Tracker
{
    /// <summary>
    /// Count Tracker counts new messages that come in.
    /// If MatchString property is optionally set, each instance of the match is counted
    /// GetResults will return of type int
    /// </summary>
    public class CountTracker : ICountTracker
    {
       
        public CountTracker(string name)
        {
            _trackerName = name;
        }

        public void OnNewMessage(TwitterStreamModel msg)
        {
            _msgCount++;

            if (MatchString != null)
            {
                var regex = new Regex(MatchString);
                var matches = regex.Matches(msg.data.text);
                if (matches.Count > 0)
                {
                    _matchCount += matches.Count;
                }
            }
        }

       

        /// <summary>
        /// Returns total message count
        /// </summary>
        /// <returns></returns>
        public object GetResults()
        {
            return _msgCount;
        }

        /// <summary>
        /// Returns count of Matches based on MatchString
        /// </summary>
        /// <returns></returns>
        public object GetTotalMatchCount()
        {
            return _matchCount;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
           
            sb.AppendLine("------------------------------------------------");
            if(MatchString==null)
                sb.AppendLine(string.Format("CountTracker Results for {0}, are Count:{1}", TrackerName, _msgCount));
             else
                sb.AppendLine(string.Format("CountTracker Results for {0}, are TotalMsgCount:{1}, TotalMatchCount:{2}", TrackerName, _msgCount,_matchCount));

            return sb.ToString();

        }

      

        private int _matchCount = 0;
        public int MatchCount { get { return _matchCount; } }

        private int _msgCount = 0;
        public int MsgCount { get { return _msgCount; } }

        private readonly string _trackerName;
        public string TrackerName { get { return _trackerName; } }

        public string MatchString { get; set; } = null;

    }


}
