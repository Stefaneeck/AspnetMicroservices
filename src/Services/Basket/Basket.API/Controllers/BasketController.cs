using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
        {
            _repository = repository;
            _discountGrpcService = discountGrpcService;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            //if basket is null, create a new shopping cart
            return Ok(basket ?? new ShoppingCart(userName));
        }

        //We are expecting the entire shoppingcart object from the body
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //consume discount grpc method (from discount.grpc) basket api will be the client of the grpc in this case
            //it's not best practice do directly communicate with the generated classes. We create a new class to encapsulate.
            foreach(var item in basket.Items)
            {
                //for every item of the basket we have an inter-service communication, because of this performance is important (postgres + dapper will help here).
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);

                //calculate new price
                item.Price -= coupon.Amount;
            }

            //action is OK, Result is return type of updatebasket (=Task<ShoppingCart>)
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
    }


}
