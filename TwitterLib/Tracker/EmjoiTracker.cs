using System;
using TwitterLib.Model;
using EmojiOne;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TwitterLib.Tracker
{

    /**
    Top emojis in tweets - GetResults()
    Percent of tweets that contains emojis - PctEmoji
    **/

    public class EmojiTracker : IEmojiTracker
    {
       

        ConcurrentBag<EmojiModel> _emojiQueue = new ConcurrentBag<EmojiModel>();

        public EmojiTracker(string trackerName)
        {
            _trackerName = trackerName;
        }


        public void OnNewMessage(TwitterStreamModel msg)
        {
          
            try
            {
                //Incriment total Message Count
                _msgCount++;

                //Parse Tweet to discover any Emojis
                List<string> emojis = EmojiOne.EmojiOne.ToEmojiValue(msg.data.text);

                if (emojis != null && emojis.Count > 0)
                {
                    //Incriment count since tweet contains emoji
                    _tweetContainEmoji++;

                    //Add Emojis to list
                    emojis.ForEach(item => _emojiQueue.Add(new EmojiModel { EmojiShortName = item.ToString() }));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public object GetResults()
        {

            var result = _emojiQueue.Cast<EmojiModel>()
                            .GroupBy(item=> item.EmojiShortName)
                            .Select(g => new EntityResult
                            {
                                Name = g.Key,
                                Count = g.Distinct().Count()

                            })
                            .OrderByDescending(item => item.Count)
                            .Take(10)
                            .ToList();

            return result as List<EntityResult>;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<EntityResult> list = ((ITracker)this).GetResults() as List<EntityResult>;
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine(string.Format("EmojiTracker Results for {0}", TrackerName));
            sb.AppendLine(string.Format("% of Tweets containing emojies: {0}", PctEmoji.ToString("P", CultureInfo.InvariantCulture)));
            sb.AppendLine("Top Items:");
            foreach (var item in list)
            {
                sb.AppendLine(string.Format("   Item: {0}, Count:{1}", item.Name, item.Count));
            }

            return sb.ToString();
        }

        private string _trackerName = null;
        public string TrackerName { get { return _trackerName; } }

        private int _msgCount = 0;
        public int MsgCount { get { return _msgCount; } }

        private int _tweetContainEmoji = 0;
        public int TweetContainEmoji { get { return _tweetContainEmoji; } }

        //Percent of tweets that contains emojis
        public double PctEmoji
        {
            get
            {
                if (MsgCount > 0)
                {
                    return TweetContainEmoji / (double)MsgCount;
                }
                return 0;
            }
        }

    }
}
