using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                //custom logging settings
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    //this is an action, the code will be executed somewhere in the extension method (ConfigureLogging)

                    //check for the logging configuration under the appsettings.json
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    //this way, we can write logs from the console and debug window and easily track ocelot logs. 
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                }
                );
    }
}
