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
            //our extension method
            //first parameter is an action, you can think of this as passing a method as parameter
            //migratedatabase does not have any knowledge about the TContext
            //Our SeedAsync method in OrderContext is expecting the OrderContext type
            //so thats way, to provide this context, we have to pass this seeding operation after the migration operation while providing the other context into the host method
            //expects an Action and an int (first parameter IHost can be ignored because it is the extension method 'this' parameter

            //context and services will get concrete values in the MigrateDatabase extension method (in HostExtensions.cs)
            host.MigrateDatabase<OrderContext>((context, services) =>
            {
                //writing the code this way (with the Action), we can place this code here instead of in the MigrateDatabase Extension method
                //Good for readability
                
                //Als je een db gaat migreren, moet je op een bepaald moment iets uitvoeren (in dit geval Logger en Seed) binnen de scope van DbContext.
                //Ik geef delegate mee om op dat moment uit te voeren. Als je delegate meegeeft aan methode, doe je dat om het op een bepaald moment te kunnen uitvoeren.

                var logger = services.GetService<ILogger<OrderContextSeed>>();

                OrderContextSeed.SeedAsync(context, logger).Wait();
            }
            );
            host.Run();

            /* this code would do the same as the code above:
             * 
             * CreateHostBuilder(args)
                .Build()
                .MigrateDatabase<OrderContext>((context, services) =>
                    {
                        var logger = services.GetService<ILogger<OrderContextSeed>>();
                        OrderContextSeed
                            .SeedAsync(context, logger)
                            .Wait();
                    })
                .Run();
             * 
             */
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
