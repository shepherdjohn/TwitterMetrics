using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Timers;
using TwitterLib.Tracker;
using static TwitterLib.Util;
using System.Text;
using TwitterService.Shared;
using TwitterService;
using TwitterLib.Model;

namespace TwitterLib
{

    /**
    Total number of tweets received -- done

    Top hashtags - done

    Percent of tweets that contain a url - done
    Percent of tweets that contain a photo url (pic.twitter.com or Instagram) - done
    Top domains of urls in tweets - done

    Average tweets per hour/minute/second - done


    Top emojis in tweets - done
    Percent of tweets that contains emojis - done

    **/




    public interface ITrackerManager
    {
        void OnNewStreamMessage(object sender, EventArgs e);
        object GetTrackerResults();

    }

    /// <summary>
    /// Manages and feeds data to the attached trackers
    /// Provides consolidated results of the trackers
    /// </summary>
    public class TrackerManager : ITrackerManager
    {

        private List<ITracker> _trackerList = new List<ITracker>();
        

        public TrackerManager(int reportingInterval)
        {

            StartTimer(reportingInterval);
            ConfigureTrackers();

        }

      
        public void OnNewStreamMessage(object sender, EventArgs e)
        {
            try
            {
                int nullCount = 0;


                if (e is MessageReceivedEventArgs)
                {
                    MessageReceivedEventArgs message = e as MessageReceivedEventArgs;

                    TwitterStreamModel model = JsonConvert.DeserializeObject<TwitterStreamModel>(message.Message.Body);
                    if (model == null)
                    {
                        nullCount++;
                    }
                    else
                    {
                        ParallelLoopResult result = Parallel.ForEach(_trackerList.ToArray(), (current) =>
                        {
                            current.OnNewMessage(model);
                        });
                    }
                }
                else
                {
                    // Receive new data and feed the data.text to each tracker using Parallel tasks for speed
                    TweetReceivedEventArgs tweet = e as TweetReceivedEventArgs;
                    ParallelLoopResult result = Parallel.ForEach(_trackerList.ToArray(), (current) =>
                    {
                        current.OnNewMessage(tweet.TwitterStreamModel);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
        }

       
        public object GetTrackerResults()
        {
            try
            {
                // Return consolidated results from each tracker
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--------------------------------------------------------------------------------");
                sb.AppendLine(string.Format("Twitter Metrics {0}", DateTime.Now.TimeOfDay));
                foreach (var item in _trackerList)
                {
                    sb.AppendLine(item.ToString());
                }
                return sb;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;

        }

        private void ConfigureTrackers()
        {
           
            RegisterObserver(new CountTracker("TotalTweets"));
            RegisterObserver(new HashtagTracker("HashtagTracker"));
            RegisterObserver(new UrlTracker("UrlTracker"));
            RegisterObserver(new EmojiTracker("EmojiTracker"));
            RegisterObserver(new RateTracker("1SecRateTracer", 1000, 1));
            RegisterObserver(new RateTracker("1MinRateTracker", 60000, 1));
            RegisterObserver(new RateTracker("1HourRateTracker", 3600000, 1));
           
        }

        /// <summary>
        /// Adds trackers to internal list
        /// </summary>
        /// <param name="tracker"></param>
        private void RegisterObserver(ITracker tracker)
        {
            _trackerList.Add(tracker);
        }

        /// <summary>
        /// Starts the timer for automatic polling of results from each tracker
        /// </summary>
        /// <param name="elapsedTime"></param>
        private void StartTimer(int elapsedTime)
        {
            if (elapsedTime > 0)
            {
                System.Timers.Timer timer;
                timer = new System.Timers.Timer(elapsedTime);
                timer.Elapsed += OnTimerTick;
                timer.Enabled = true;
                timer.AutoReset = true;
                timer.Start();
            }
        }

        private void OnTimerTick(Object source, ElapsedEventArgs e)
        {
            
            Console.WriteLine(GetTrackerResults());
        }

      
       
    }
}
