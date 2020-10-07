using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitterService.Entities;
using TwitterService.Services;

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

                    services.AddSingleton<IMessageChannel, MessageChannel>();
                    services.AddHostedService<ChannelProducerService>();
                    services.AddHostedService<ChannelConsumerService>();
                    
                });
    }
}
