using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    //Discount.API is not used, it's just made to show what is possible. We use Discount GRPC.
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _repository;

        public DiscountController(IDiscountRepository repository)
        {
            _repository = repository;
        }

        //{productName} means that the productName parameter in the method will be filled by what we get from the api get request.
        [HttpGet("{productName}", Name = "GetDiscount")]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _repository.GetDiscount(productName);
            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            await _repository.CreateDiscount(coupon);
            //by using CreatedAtRoute we are calling the GetDiscount method
            //we can see all details after inserting the coupon by working this way
            //https://stackoverflow.com/questions/25045604/can-anyone-explain-createdatroute-to-me/25110700

            /*
             *
            When you use CreatedAtRoute, the first argument is the route name of the GET to the resource.
            The trick that is not so obvious is that, even with the correct method name specified, you must thus use the Name param on the HttpGet attribute for it to work.
            So if the return in your POST is this:
            return CreatedAtRoute("Get", routeValues: new { id = model.Id }, value: model);
            Then your Get method attribute should look like this even if your method is named Get:
            [HttpGet("{id}", Name = "Get")]
            Calls to your Post method will not only return the new object (normally as JSON), it will set the Location header on the response to the URI that would Get that resource.
            So if you POST an order item for instance, you might return a route like 'api/order/11'

            NOTE the field names in the routeValues field names need to match the binding names in the target route, i.e. there needs to be a field named id to match the {id} in HttpGet("{id}"
             */
            return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
        {
            return Ok(await _repository.UpdateDiscount(coupon));
        }

        [HttpDelete("{productName}", Name = "DeleteDiscount")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string productName)
        {
            return Ok(await _repository.DeleteDiscount(productName));
        }
    }
}
