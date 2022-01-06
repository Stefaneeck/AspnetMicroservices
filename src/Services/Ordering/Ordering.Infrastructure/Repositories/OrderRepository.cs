using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    //inherits the common methods (in order to perform basic CRUD operations)
    //and inherits the order specific methods
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        //Order specific methods here

        /*we have injected dbContext in RepositoryBase
         * When you have a base class, a constructor with the parameter
           you have to add this constructor parameter method in the subclass
         */
        public OrderRepository(OrderContext dbContext) : base(dbContext)
        {

        }
        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            //_dbContext is accessible from RepositoryBase because we have given it the protected access modifier
            var orderList = await _dbContext.Orders
                                    .Where(o => o.UserName == userName)
                                    .ToListAsync();
            return orderList;
        }
    }
}
