using Catalog.API.Data.Repositories;
using Catalog.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /*
         * In ASP.NET Core the former MVC "Controller"-class and Web Api "ApiController"-class where merged together to one "Controller"-class. 
         * The ActionResult method, which is a classic MVC method and useful when building an MVC application, 
         * where also included in that merged and that's why it's being available when also building a Web Api.
         * 
         * If you notice the prebuild project of the .net core web api, not all functions return ActionResult or IActionResult.
         * So yes, you can totally return something like CustomerDto. This way you can return an object as json, with the status code 200. 
         * However let's say your action is taking in some input and you are not always sure if there will be a valid output. In this case 
         * you will want to return status code 200 only if successful, 400 (badRequest) if the user sent invalid data or any other status code. This is the standard way of handling http requests.

           So, by using IActionResult you can either return a CustomerDto object by using return Json(customer) or return Ok(customer), or you can return use BadRequest(myErrors) when you run into errors.
         */

        [HttpGet]
        //ProduceResponseType is for producing Open API metadata for API exploration/visualization tools such as Swagger, to indicate in the documentation what the controller could return.
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id: {id} not found");
                return NotFound();
            }
            return Ok(product);
        }

        //>1 httpget in 1 controller -> differentiate by using httpget name ="" or route
        //[action] is the method name, in this case GetProductByCategory, so url will be GetProductByCategory/{category}
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.CreateProduct(product);

            /*
             * After creation, give status 201 and forward to GetProduct with the new created product id as parameter.
             * 
             * When you use CreatedAtRoute, the first argument is the route name of the GET to the resource. 
             * The trick that is not so obvious is that, even with the correct method name specified, 
             * you must use the Name param on the HttpGet attribute for it to work.
             */
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        //IActionResult if you dont want to return any specific type of response
        //We can use IActionResult here instead of ActionResult because we're not returning a specific type (ex. ActionResult<Product>).
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }
    }
}
