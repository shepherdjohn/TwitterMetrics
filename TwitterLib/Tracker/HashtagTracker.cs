using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitterLib.Model;

namespace TwitterLib.Tracker
{
    /**

    Top hashtags - GetResults()

    **/

    public class HashtagTracker : EntitiesTrackerBase
    {
        public HashtagTracker(string name) : base(name)
        {

        }

        protected override void HandleMessage(TwitterStreamModel model)
        {
            List<Hashtag> objectList = model.data.entities.hashtags;

            if (objectList != null)
            {
                foreach (var value in objectList)
                {
                    _listQueue.Add(value);
                }
            }
        }

        public override object GetResults()
        {

            var result = _listQueue.Cast<Hashtag>()
                            .GroupBy(l => l.tag)
                            .Select(g => new EntityResult
                            {
                                Name = g.Key,
                                Count = g.Distinct().Count()

                            })
                            .OrderByDescending(item => item.Count)
                            .Take(ReturnTopItems)
                            .ToList();


            return result.Cast<EntityResult>();

        }



    }
}
