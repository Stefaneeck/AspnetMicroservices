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
                //look for the ocelot json file when the application starts
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    //basic way
                    //config.AddJsonFile("ocelot.json");

                    //this will choose our ocelot.Development or ocelot.Local depending on which value is set for the ASPNETCORE_ENVIRONMENT environment variable, in the project properties.
                    //HostingEvironment.EnvironmentName indicates in which environment you are working (development, local,..).
                    config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                //custom logging settings to show the ocelot logs in the console
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    //this is an action, the code will be executed somewhere in the extension method (ConfigureLogging)
                    //we only have to say what needs to be executed. Since these are built-in methods we can't exactly see how the values get set etc (we only can see the extension method signature, not the implementation details).

                    //check for the logging configuration under the appsettings.json
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    //this way, we can read logs from the console and debug window to easily track ocelot logs. 
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();

                });
    }
}
