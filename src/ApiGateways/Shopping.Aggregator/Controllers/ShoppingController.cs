using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Net;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    //exposes an api to the client application in order to consume the shopping model webservices
    //we could extend our microservices, for instance create a new api gateway for implementing the backend for frontend for a mobile application our an angular spa 
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderService orderService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _orderService = orderService;
        }

        //returns basket, order and the product related items in one model
        //this is the one service that we will expose, this will aggregate our results into one response
        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task <ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            //get basket with username (consume basket microservice)
            var basket = await _basketService.GetBasket(userName);

            //iterate basket items and consume products (get catalog info of specific basket items) with basketitem ProductId 
            foreach(var basketItemExtended in basket.Items)
            {
                var catalogItem = await _catalogService.GetCatalog(basketItemExtended.ProductId);

                //map catalog product related members into basketitem dto with extended columns
                basketItemExtended.ProductName = catalogItem.Name;
                basketItemExtended.Category = catalogItem.Category;
                basketItemExtended.Summary = catalogItem.Summary;
                basketItemExtended.Description = catalogItem.Description;
                basketItemExtended.ImageFile = catalogItem.ImageFile;
            }

            //consume ordering microservice in order to retrieve orderlist
            var orders = await _orderService.GetOrdersByUserName(userName);

            //return root shopping modelclass (dto class ShoppingModel which includes all responses)
            var shoppingModel = new ShoppingModel
            {
                BasketWithProducts = basket,
                Orders = orders,
                UserName = userName
            };

            return Ok(shoppingModel);
        }
    }
}
