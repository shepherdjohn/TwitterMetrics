using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TwitterLib.Model;
using static TwitterLib.Util;

namespace TwitterLib.Tracker
{



    public abstract class EntitiesTrackerBase : IEntitiesTracker
    {


        protected ConcurrentBag<IEntity> _listQueue = new ConcurrentBag<IEntity>();

        public EntitiesTrackerBase(string name)
        {
            _trackerName = name;
            
        }

        public void OnNewMessage(TwitterStreamModel msg)
        {
            _msgCount++;

            if (msg.data.entities == null)
                return;

            HandleMessage(msg);

        }

        protected abstract void HandleMessage(TwitterStreamModel model);

       
        public abstract object GetResults();


        // public abstract override string ToString();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var list = GetResults() as List<EntityResult>;
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine(string.Format("EntityTracker Results for {0}", TrackerName));
            sb.AppendLine("Top Items:");
            foreach (var item in list)
            {
                sb.AppendLine(string.Format("   Item: {0}, Count:{1}", item.Name, item.Count));
            }

            return sb.ToString();
        }


        protected int _msgCount = 0;
        /// <summary>
        /// Tracks the total number of messages received
        /// </summary>
        public int MsgCount { get { return _msgCount; } }

        protected int _msgWithEntityCount = 0;
        public int MsgWithEntityCount { get { return _msgWithEntityCount; } }

        protected readonly string _trackerName;
        public string TrackerName { get { return _trackerName; } }

        public int QualifiedMsgCount { get { return _listQueue.Count; } }

        /// <summary>
        /// Set the number of items to return in GetResults, default is 10
        /// </summary>
        public int ReturnTopItems { get; set; } = 10;
    }
}
