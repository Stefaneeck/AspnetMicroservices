using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    //gets data from the mongodb
    public class CatalogContext : ICatalogContext
    {
        //aspnetcore built-in DI (used for configuration)
        public CatalogContext(IConfiguration configuration)
        {
            //gets connectionstring value from appsettings.json
            var client = new MongoClient(configuration.GetValue<string> ("DatabaseSettings:ConnectionString"));

            //if there is no db, it will create one for us
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            //populate products
            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));

            //seed data
            //CatalogContextSeed.SeedData(Products);
        }
        public IMongoCollection<Product> Products { get; }
    }
}
