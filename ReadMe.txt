This is a .net Core 3.1 application

Main classes used for listening to the Twitter feed are:
1. Twitter Producer
2. Twitter Consumer

The TwitterProducer listens to the twitter feed and when messages are fed to the producer, the messages are then put onto a Channel Queue to be read by a consumer
The Twitter Consumer listens to the channel queue for new messages and notifies "trackers" when a message arrives

Main Classes for calculating metrics
1. TrackerManager

The TrackerManager is responsible for instantiating new "trackers" that will be used to gather metrics on each Tweet received by the Twitter Consumer

Classes for Trackers:
1. RateTracker - Rate tracker takes an interval in the consturctor and configures a timer that would end each "slice". When new messages
    arrive, the current slice is incrimented. when the interval period expires, the current slice is put into a queue and a new current
    slice is created.
    Improvements - This could be converted to use the twitter msg time to compute when slices begin and end

2. CountTracker - Count tracker counts incoming messages. There is an optional variation which will count messages only if a regex match is found
3. EntitiesTracker - Tracks entity information from the Twitter Feed. Entities include, Url's, Hashtags, Mentions & Annotations
    Entities Tracker contains the following classes:
    1. EntityTrackerBase - Abstract class
    2. HashtagTracker - Tracker to track hashtag metrics
    3. UrlTracker - Tracker to track URL metrics. Expanded to also track if URL is a link to a Photo or Video

    Interfaces:
    All trackers inherit from ITracker
    Other Interfaces are also created to add functionality specifically to enhance the usability for the trackers

Reporting Metrics:
The TrackerManager has a timer that when fired calls each Trackers overridden .ToString() method which individually produce their metric and then
the TrackerManager consolidates each message for return to the user

Sample Metrics:

    Twitter Metrics 15:13:01.6768170
    ------------------------------------------------
    CountTracker Results for TotalTweets, are Count:3611

    ------------------------------------------------
    EntityTracker Results for HashtagTracker
    Top Items:
       Item: PueblaSinDerechos, Count:20
       Item: بندر_بن_سلطان, Count:19
       Item: KCAMexico, Count:12
       Item: PCAs, Count:6
       Item: GasolinaParaElPueblo, Count:5
       Item: CNCO, Count:5
       Item: Bia, Count:5
       Item: BB14, Count:5
       Item: BiggBoss14, Count:5
       Item: COVID19, Count:4

    ------------------------------------------------
    EntityTracker Results for UrlTracker
    Percent of tweets that contain a url:29.30 %
    % Tweets with Photos: 14.32 %
    Top Items:
       Url:twitter.com, Count:1155
       Url:bit.ly, Count:8
       Url:youtu.be, Count:8
       Url:www.instagram.com, Count:7
       Url:twcm.co, Count:6
       Url:www.facebook.com, Count:5
       Url:onlyfans.com, Count:4
       Url:www.youtube.com, Count:4
       Url:briefly.co, Count:3
       Url:headcount.org, Count:3

    ------------------------------------------------
    EmojiTracker Results for EmojiTracker
    % of Tweets containing emojies: 21.93 %
    Top Items:
       Item: :joy:, Count:237
       Item: :sparkling_heart:, Count:159
       Item: :nauseated_face:, Count:102
       Item: :sob:, Count:88
       Item: :heart:, Count:83
       Item: :rofl:, Count:63
       Item: :pleading_face:, Count:62
       Item: :face_vomiting:, Count:44
       Item: :sparkles:, Count:42
       Item: :heart_eyes:, Count:29

    ------------------------------------------------
    RateTracker Interval:1000, MsgRate:103.85

    ------------------------------------------------
    RateTracker Interval:60000, MsgRate:53.4

    ------------------------------------------------
    RateTracker Interval:3600000, MsgRate:0

