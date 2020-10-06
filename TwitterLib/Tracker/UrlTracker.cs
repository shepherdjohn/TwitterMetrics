using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TwitterLib.Model;

namespace TwitterLib.Tracker
{
    public class UrlTracker : EntitiesTrackerBase, IUrlTracker
    {
        /**
        Percent of tweets that contain a url - UrlPercent
            UrlPercent returns the total number of tweets that contain at least 1 URL / Total Received Tweets
            Assumption: Will only store a value of 1 per tweet regarless of how many URL's may be in the tweet
            

        Percent of tweets that contain a photo url(pic.twitter.com or Instagram) - PhotoPct
            PhotoPct returns the total number of tweets that contain at least 1 Photo / Total Received Tweets
            Assumption: Will only store a value of 1 per tweet regarless of how many URL's may be in the tweet

        Top domains of urls in tweets - GetResults() Method
            GetResults will search all received URL's and will return a list of every distinct occurance
            of the URL as well as the number of times that URL appeared in a tweet
            Assumption: Uses all URL's received in a tweet

        Additional Metrics:

        Total Photos Count - PhotoCount
            Returns the total number of URL's that are photos that were received

        PctPhotosInTweet - PhotosTweetPct
            Returns the percentage of Photos per every Tweet


        **/

        public UrlTracker(string name) : base(name)
        {

        }

        /// <summary>
        /// Returns the top (ReturnTopItems) of Domains along with the total distinct count
        /// </summary>
        /// <returns></returns>
        public override object GetResults()
        {
            var result = _listQueue.Cast<Url>()
                           .GroupBy(l => l.Domain)
                           .Select(g => new EntityResult
                           {
                               Name = g.Key,
                               Count = g.Distinct().Count()

                           })
                           .OrderByDescending(g => g.Count)
                           .Take(ReturnTopItems)
                           .ToList();

            return result.Cast<EntityResult>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var TopTweets = GetResults() as List<EntityResult>;
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine(string.Format("EntityTracker Results for {0}", TrackerName));
            //Percent of tweets that contain a url - UrlPercent
            sb.AppendLine(string.Format("Percent of tweets that contain a url:{0}", UrlPercent.ToString("P", CultureInfo.InvariantCulture)));

            //Percent of tweets that contain a photo url(pic.twitter.com or Instagram)
            sb.AppendLine(string.Format("% Tweets with Photos: {0}", PhotoPct.ToString("P", CultureInfo.InvariantCulture)));

            //Top Domains
            sb.AppendLine("Top Items:");
            foreach (var item in TopTweets)
            {
                sb.AppendLine(string.Format("   Url:{0}, Count:{1}", item.Name, item.Count));
            }
            return sb.ToString();

        }

        protected override void HandleMessage(TwitterStreamModel model)
        {
            // Get a list of each Url listed in the tweet entities url list
            List<Url> urlList = model.data.entities.urls;

            if (urlList != null)
            {
                //Incriment TweetsWithUrls as there is at least one in the collection
                _tweetWithUrl++;

                // Check if this tweet contained any photos
                if (urlList.Any(item => item.IsPhoto))
                {
                    _tweetWithPhoto++;
                }

                //Add each Url to the internal list of Urls to track top Url's
                foreach (var value in urlList)
                {
                    _listQueue.Add(value);

                    //see if the Url is a photo
                    if (value.IsPhoto)
                    {
                        _totalPhotoCount++;
                    }
                }
              
            }
        }

        private int _tweetWithUrl = 0;
        /// <summary>
        /// Returns total number of tweets that contain any number of Url's, as long as there is 1 Url in the tweet
        /// </summary>
        public int TweetsWithUrl { get { return _tweetWithUrl; } }

        public double UrlPercent
        {
            get
            {
                if (MsgCount > 0)
                {
                    return TweetsWithUrl / (double)MsgCount;
                }
                return 0;
            }
        }

        private int _tweetWithPhoto = 0;
        /// <summary>
        /// Returns total number of tweets that contain any number of photos, as long as there is 1 photo in the tweet
        /// </summary>
        public int TweetsWithPhotos { get { return _tweetWithPhoto; } }

        private int _totalPhotoCount = 0;
        /// <summary>
        /// Returns total number of Photos
        /// Provided a tweet contains 2 photos, this count would = 2
        /// </summary>
        public int PhotoCount { get { return _totalPhotoCount; } }

        /// <summary>
        /// Returns the Percentage of Tweets with Photos to total Tweets received
        /// </summary>
        public double PhotoPct
        {
            get
            {
                if (MsgCount > 0)
                {
                    return TweetsWithPhotos / (double)MsgCount;
                }
                return 0;
            }
        }

        public double PhotosTweetPct
        {
            get
            {
                if (MsgCount > 0)
                {
                    return PhotoCount / (double)TweetsWithPhotos;
                }
                return 0;
            }
        }

       
    }
}
