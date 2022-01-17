using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        //Generic, should we not do this then we should write a MigrateDatabase method for each DbContext
        public static IHost MigrateDatabase<TContext>(this IHost host,
                                            Action<TContext, IServiceProvider> seeder,
                                            int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;

            //This will provide the logger and the context object
            //we can access these trough the var scope
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                //type of seeder methods first parameter is TContext
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    //without .Name it would return meta-information about TContext.class
                    logger.LogInformation("Migrating database with context {DbContextName}", typeof(TContext).Name);

                    /* The code that is actually executed here comes from Program.cs and is:
                     * 
                    {
                        var logger = services.GetService<ILogger<OrderContextSeed>>();
                        OrderContextSeed.SeedAsync(context, logger).Wait();
                    }
                     * 
                     */
                    InvokeSeeder(seeder, context, services);

                    logger.LogInformation("Migrated database with context {DbContextName}", typeof(TContext).Name);

                }
                catch (SqlException ex)
                {

                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);

                    //retry because we are dependent on other containers which may not be ready yet
                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, seeder, retryForAvailability);
                    }
                }

            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
                                                    TContext context,
                                                    IServiceProvider services) where TContext : DbContext
        {
            //will create the db if needed and run the migrations
            context.Database.Migrate();
            //seed db, Action type (method as parameter), we dont know the generic type in this extension class
            seeder(context, services);
        }
    }
}
