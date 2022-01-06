using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        //We create mediator cqrs objects and send these requests to the mediator.
        //Behind this action, mediator creates a pipeline for the request and triggers the handler method
        //no infrastructure dependencies, presentation layer is only responsible for exposing API

        //for all operations only 1 object to inject
        //mediator is not referenced directly in our project file, but we have a project reference to Ordering.Application, which has the MediatR reference.
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userName}", Name = "GetOrder")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetOrdersByUserName(string userName)
        {
            var query = new GetOrdersListQuery(userName);
            //send query object into mediator, mediator will handle via handler class and retrieve from db there (no respository is neccessary here)
            //mediator makes our methods very simple and clean
            //we don't need to worry about the business objects, these will be handled in the Ordering.Application
            var orders = await _mediator.Send(query);

            return Ok(orders);
        }

        //Testing
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /* swagger documentation:
         * 
         * An API specification needs to specify the responses for all API operations. Each operation must have at least one response defined, usually a successful response. A response is defined by its HTTP status code and the data returned in the response body and/or headers.
         * Sometimes, an operation can return multiple errors with different HTTP status codes, but all of them have the same response structure.
         * You can use the default response to describe these errors collectively, not individually. “Default” means this response is used for all HTTP codes that are not covered individually for this operation.
         * 
         */
        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //a code path in UpdateOrderCommandHandler throws a NotFoundException
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]

        public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]

        public async Task<ActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand() { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

    }
}
