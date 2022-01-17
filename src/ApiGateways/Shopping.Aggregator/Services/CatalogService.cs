using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        #region commentinfo
        //this http client comes from the HttpClientFactory
        //a standard http client for CatalogService has been created by using our configuration in ConfigureServices
        //this includes the base address (it is set in ConfigureServices)
        //by using the httpclientfactory, all info (such as base address for requests, retry policy,..) that has been given in the ConfigureServices method doesnt need to be configured here

        //services.AddHttpClient<ICatalogService, CatalogService>(c =>
        //c.BaseAddress = new Uri(Configuration["ApiSettings:CatalogUrl"]));
        //the value of CatalogUrl in appsettings is http://localhost:8000
        //The _client in this class will already have a base address, we can send a request by entering "/api/v1/Catalog" for instance.
        #endregion

        private readonly HttpClient _client;

        public CatalogService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            //sends get request
            var response = await _client.GetAsync("/api/v1/Catalog");
            //map json to catalogmodel
            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            var response = await _client.GetAsync($"/api/v1/Catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var response = await _client.GetAsync($"/api/v1/Catalog/GetProductByCategory/{category}");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
    }
}
