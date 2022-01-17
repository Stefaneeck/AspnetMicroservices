using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Extensions;
using Ordering.Infrastructure.Persistence;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            //because the containers should create their own dbs and data, we apply migrations automatically when ordering api starts up
            var host = CreateHostBuilder(args).Build();

            #region commentextension
            /*our extension method
            first parameter is an action, you can think of this as passing a method as parameter
            migratedatabase does not have any knowledge about the TContext
            Our SeedAsync method in OrderContext is expecting the OrderContext type
            so that way, to provide this context, we have to pass this seeding operation after the migration operation while providing the other context into the host method
            expects an Action and an int (first parameter IHost can be ignored because it is the extension method 'this' parameter
            */
            #endregion

            //context and services will get concrete values in the MigrateDatabase extension method (in HostExtensions.cs)
            host.MigrateDatabase<OrderContext>((context, services) =>
            {
                //writing the code this way (with the Action), we can place this code here instead of in the MigrateDatabase Extension method, good for readability
                //when migrating a db, at a certain point in time something needs to be executed (in our case Logger and Seed) inside the scope of DbContext.
                //We pass a delegate, which will be executed at that moment. When you pass a delegate to a method, you do that in order to run that code on a specific moment.

                var logger = services.GetService<ILogger<OrderContextSeed>>();

                OrderContextSeed.SeedAsync(context, logger).Wait();
            }
            );
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
