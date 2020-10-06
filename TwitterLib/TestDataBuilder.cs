using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TwitterLib.Model;
using static TwitterLib.Util;

namespace TwitterLib
{
    public class TestDataBuilder : ITrackerManager
    {

        List<TwitterStreamModel> _tweets = new List<TwitterStreamModel>();

        public TestDataBuilder()
        {



        }

        public object GetTrackerResults()
        {
            return null;
        }

        public void OnNewStreamMessage(object sender, EventArgs e)
        {
            TweetReceivedEventArgs msg = (TweetReceivedEventArgs)e;
            _tweets.Add(msg.TwitterStreamModel);

            if(_tweets.Count >= MsgToWrite && !_writeToFileComplete)
            {
                WriteToFile();
            }

        }


        private void WriteToFile()
        {
            try
            {

                _writeToFileComplete = true;
                using (StreamWriter file = File.CreateText(@"LargeJsonTestData.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, _tweets);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _writeToFileComplete = false;
            }
            
        }

        /// <summary>
        /// Determine the number of messages to write to file
        /// </summary>
        public int MsgToWrite { get; set; } = 1000;

        private bool _writeToFileComplete = false;
    }
}
