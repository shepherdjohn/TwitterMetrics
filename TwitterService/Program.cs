using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterService.Entities;
using TwitterService.Services;
using TwitterLib;
using Microsoft.Extensions.Configuration;
using TwitterService.Shared;

namespace TwitterService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    services.Configure<FeaturesConfiguration>(hostContext.Configuration.GetSection("Features"));
                    services.AddSingleton<ITrackerManager, TrackerManager>();
                    services.AddSingleton<IMessageChannel, MessageChannel>();
                    services.AddHostedService<ChannelProducerService>();
                    services.AddHostedService<ChannelConsumerService>();
                    
                });
    }

   
}
