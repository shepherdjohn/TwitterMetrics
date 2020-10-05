using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TwitterLib.Model;

namespace TwitterLib.Tracker
{


    /// <summary>
    /// RateTracker measures the average number of messages received
    /// A Slice represents is a single interval of time specified by the Interval
    /// The AvgPeriod is how many Slices will be used to compute the avgMessageRate
    /// </summary>
    public class RateTracker : IRateTracker
    {

        private Slice currentSlice;
        private ConcurrentBag<Slice> slices = new ConcurrentBag<Slice>();
        private System.Timers.Timer timer = null;

     

        public RateTracker(string trackerName, int interval, int avgPeriod)
        {
            TrackerName = trackerName;
            _interval = interval;
            _avgPeriod = avgPeriod;
        }

        public void OnNewMessage(TwitterStreamModel msg)
        {
            OnNewMessage(msg.data.text);
        }

        public void OnNewMessage(string msg)
        {
            if (currentSlice == null)
                currentSlice = new Slice(Interval);

            if (timer == null)
                StartTimer(Interval);

            currentSlice.IncrementCount();
        }

        public object GetResults()
        {
            return GetAvgRate(AvgPeriod);
        }

        public override string ToString()
        {
            var msgRate = GetResults();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------");
            sb.AppendLine(string.Format("RateTracker Interval:{0}, MsgRate:{1}", Interval, decimal.Round((decimal)msgRate, 2)));
            return sb.ToString();
        }


        private void StartTimer(int interval)
        {

            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnTimerTick;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
        }

        private void OnTimerTick(Object source, ElapsedEventArgs e)
        {
            if (currentSlice != null)
            {
                currentSlice.EndSlice();
                slices.Add(currentSlice);
                currentSlice = null;
            }
        }

        private decimal GetAvgRate(int periods)
        {
          
            if (slices.Count < periods)
                return 0;

            var slicePeriods = (from t in slices
                         select t).Take(periods);

            int count = 0;
            foreach(var item in slicePeriods)
            {
                count = count + item.Count;
            }

            var elapsedTime =  slicePeriods.First().EndTime - slicePeriods.Last().StartTime ;


            var time = decimal.Round((decimal)elapsedTime.Ticks / 10000000,2);

            decimal rate = count / time;
            return rate;
        }

      

        private class Slice
        {
      
            internal Slice(int elapsed)
            {
                _startTime = DateTime.Now.TimeOfDay;
              
            }

            internal void IncrementCount()
            {
                _count++;
            }

            internal void EndSlice()
            {
                EndTime = DateTime.Now.TimeOfDay;
            }

            internal TimeSpan ElapsedTime
            {
                get
                {
                    if(EndTime==TimeSpan.MinValue)
                    {
                        return DateTime.Now.TimeOfDay - StartTime;
                    }
                    else
                    {
                        return EndTime - StartTime;
                    }
                }
            }

            TimeSpan _startTime;
            internal TimeSpan StartTime { get { return _startTime; } }

            TimeSpan _endTime;
            internal TimeSpan EndTime { get { return _endTime; } private set { _endTime = value; } }

            int _count = 0;
            internal int Count { get { return _count; } }
        }


        private readonly int _interval;
        public int Interval { get { return _interval; } }

        private readonly int _avgPeriod;
        public int AvgPeriod { get { return _avgPeriod; } }

        public string TrackerName { get; private set; }

        private int _msgCount = 0;
        public int MsgCount { get { return _msgCount; } }
    }
}
