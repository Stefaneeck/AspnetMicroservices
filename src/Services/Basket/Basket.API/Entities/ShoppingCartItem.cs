using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; }

        public string Color { get; set; }

        public decimal Price { get; set; }

        public string ProductId { get; set; }

        //We consume an api to communicate between microservices, double data because otherwise we need to consume the api only for the product name.
        public string ProductName { get; set; }
    }
}
