using System.Collections.Generic;

namespace Shopping.Aggregator.Models
{
    //will combine our other models
    //will be our final model class that is used for responding to the client application, after they send a request with a given username
    public class ShoppingModel
    {
        public string UserName { get; set; }
        public BasketModel BasketWithProducts { get; set; }
        public IEnumerable<OrderResponseModel> Orders { get; set; }
    }
}
