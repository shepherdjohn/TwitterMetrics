using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterLib;
using TwitterLib.Model;
using TwitterLib.Tracker;
using EmojiOne;
using Newtonsoft.Json;
using System.IO;

namespace TrackerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ValidateCounterTracker()
        {
            var testData = LoadTestData();
            var testDataCount = testData.Count;

            CountTracker countTracker = new CountTracker("CountTrackerTest");
            foreach (var item in testData)
            {
                countTracker.OnNewMessage(item);
            }
        
            Assert.AreEqual(testDataCount,(int)countTracker.GetResults());

        }


        [TestMethod]
        public void ValidateHashTracker()
        {
            List<TwitterStreamModel> testModel = LoadTestData();

            var model = testModel[1];
            ITracker tracker = new HashtagTracker("TestHashtagTracker");
            tracker.OnNewMessage(model);

            var result = tracker.GetResults() as List<EntityResult>;

            Assert.AreEqual(model.data.entities.hashtags.Count, result.Count);

        }

      

        [TestMethod]
        public void ValidateUrlTracker()
        {
            List<TwitterStreamModel> testModel = LoadTestData();

            var model = testModel[0];
            IUrlTracker tracker = new UrlTracker("TestUrlTracker");
            tracker.OnNewMessage(model);

            var result = tracker.GetResults() as List<EntityResult>;
            Assert.AreEqual(model.data.entities.urls.Count, result[0].Count);

        }



        [TestMethod]
        public void ValidateRateTracker()
        {

            RateTracker rateTracker = new RateTracker("RateTrackerTest", 1000,1); //1 second interval
            for (int i = 0; i < 100; i++)
            {
                rateTracker.OnNewMessage(string.Format("Test string {0}", i));
                Thread.Sleep(50);
            }

            var avg = rateTracker.GetResults();
            Assert.IsTrue((decimal)avg >= 19);
            
        }

        [TestMethod]
        public void ParseEmojiTest()
        {
            string unicodeString = "Be Ready For Grand Welcome 😂❤️";
            List<string> emojiList = EmojiOne.EmojiOne.ToEmojiValue(unicodeString);
            Assert.AreEqual(2, emojiList.Count);
        }

        [TestMethod]
        public void ValidateEmojiTracker()
        {
            List<TwitterStreamModel> testModel = LoadTestData();

            var model = testModel[0];
            ITracker tracker = new EmojiTracker("TestEmojiTracker");
            tracker.OnNewMessage(model);

            var result = tracker.GetResults() as List<EntityResult>;
            Assert.AreEqual(1, tracker.MsgCount);


        }

        [TestMethod]
        public void ValidateEmojiTrackerLargeDataSet()
        {
            List<TwitterStreamModel> testModel = LoadLargeTestData();
            ITracker tracker = new EmojiTracker("TestEmojiTracker");
            testModel.ForEach(item => tracker.OnNewMessage(item));
     
            var result = tracker.GetResults() as List<EntityResult>;
            Assert.AreEqual(testModel.Count, tracker.MsgCount);

        }



        private List<TwitterStreamModel> LoadTestData()
        {
            List<TwitterStreamModel> models = JsonConvert.DeserializeObject<List<TwitterStreamModel>>(File.ReadAllText(@"JSONTestData.json"));
            return models;
        }

        private List<TwitterStreamModel> LoadLargeTestData()
        {
            List<TwitterStreamModel> models = JsonConvert.DeserializeObject<List<TwitterStreamModel>>(File.ReadAllText(@"LargeJsonTestData.json"));
            return models;
            
        }
    }

}
