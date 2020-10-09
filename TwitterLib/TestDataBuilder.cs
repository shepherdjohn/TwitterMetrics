using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TwitterLib.Model;
using TwitterService.Shared;


namespace TwitterLib
{
    /// <summary>
    /// Class to listen to the twitter consumer as an ITrackerManager for the purpose of generating live test data that
    /// would be saved to a file for use in further testing.
    /// </summary>
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

        public bool Initialize()
        {
            return true;
        }

        public void OnNewStreamMessage(object sender, EventArgs e)
        {
            MessageReceivedEventArgs message = e as MessageReceivedEventArgs;
            TwitterStreamModel model = JsonConvert.DeserializeObject<TwitterStreamModel>(message.Message.Body);
           
            _tweets.Add(model);

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
