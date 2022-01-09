using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //redis configuration
            services.AddStackExchangeRedisCache(options =>
            {
                //get cachesettings -> connectionstring from appsettings.json
                options.Configuration = Configuration.GetValue<string>("CacheSettings:ConnectionString");
            });

            //general configuration
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddAutoMapper(typeof(Startup));

            //grpc configuration
            //register the generated grpc client (we need the address of the discount grpc), this is the address of the using when making the grpc calls
            //the address has been set in the appsettings.json file rather than entering as string here
            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options => options.Address = new Uri(Configuration["GrpcSettings:DiscountUrl"]));

            //register for DI
            services.AddScoped<DiscountGrpcService>();

            //MassTransit-RabbitMQ configuration, configure masstransit to connect with rabbitmq
            //config is an action object
            services.AddMassTransit(config => {
                //UsingRabbitMq needs an action object as well
                config.UsingRabbitMq((ctx, cfg) => {
                    //docker evironment is exposing this port into local, rabbitmq is running on local
                    //for details check rabbitmq.com/dotnet-api.guide.html
                    //from appsettings.json
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
