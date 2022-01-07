using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,
                                            Action<TContext, IServiceProvider> seeder,
                                            int? retry = 0) where TContext : DbContext
        {
            //Value because nullable type
            int retryForAvailability = retry.Value;

            //This will provide the logger and the context object
            //we can access these trough the var scope
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database with context {DbContextName}", typeof(TContext).Name);

                    InvokeSeeder(seeder, context, services);

                    //if the sql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for DbContext only 
                    //apply to transient exceptions                    
                    //retry.Execute(() => InvokeSeeder(seeder, context, services));

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
            //needs to be returned because it's an extension method (chaining)
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
                                                    TContext context,
                                                    IServiceProvider services) where TContext : DbContext
        {
            //will create the db if needed and run the migrations
            context.Database.Migrate();
            //seed db
            //Action type (method as parameter), we dont know the generic type in this extension class
            seeder(context, services);
        }
    }
}
