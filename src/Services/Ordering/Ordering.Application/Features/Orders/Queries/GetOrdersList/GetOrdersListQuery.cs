using MediatR;
using System.Collections.Generic;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    /*
    filter by UserName
    IRequest<ResponseType>: the response type of this request is placed between <>
    OrdersVm = DTO
    We use mediator for the CQRS design pattern implementation
    mediator will trigger handler class when the request is placed
    */

    public class GetOrdersListQuery : IRequest<List<OrdersVm>>
    {
        public GetOrdersListQuery(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; set; }
    }
}
