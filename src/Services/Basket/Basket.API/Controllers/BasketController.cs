using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _repository = repository;
            _discountGrpcService = discountGrpcService;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        /*
         * If you notice the prebuild project of the .net core web api, not all functions return ActionResult or IActionResult.
         * So yes, you can totally return something like CustomerDto. This way you can return an object as json, with the status code 200.
         * However let's say your action is taking in some input and you are not always sure if there will be a valid output. 
         * In this case you will want to return status code 200 only if successful, 400 (badRequest) if the user sent invalid data or any other status code. 
         * This is the standard way of handling http requests.

         * So, by using IActionResult you can either return a CustomerDto object by using return Json(customer) or return Ok(customer), or you can return use BadRequest(myErrors) when you run into errors.
         * */
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

        //When we are calling the basket controller, we are required to write Checkout (the action) at the end of the url. This will seperate this method from the httppost method UpdateBasket. 
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //get existing basket with its total price, username should have a basket in our redis db
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
            {
                return BadRequest();
            }
            //create basketcheckoutevent, set totalprice on basketCheckout eventMessage
            //not using a basketcheckoutevent in the frombody (would save one mapping), but this event belongs to rabbitmq operations and not api methods.
            //send basketcheckoutevent to rabbitmq
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);

            eventMessage.TotalPrice = basket.TotalPrice;
            //publish message to all subscribed consumers
            //this will trigger BasketCheckoutConsumer (Ordering.API)
            await _publishEndpoint.Publish<BasketCheckoutEvent>(eventMessage);

            //remove the basket in the redis db in order to create a new fresh sales operation.
            await _repository.DeleteBasket(basket.UserName);

            return Accepted();
        }
    }


}
