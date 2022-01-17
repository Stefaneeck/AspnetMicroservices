﻿using Discount.Grpc.Protos;
using System;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    //it's best practice to not directly communicate with the generated classes.
    //this class to encapsulate the grpc client class that is generated by VS.
    public class DiscountGrpcService
    {
        //DiscountProtoService.DiscountProtoServiceClient is the class generated by VS, we inject this class
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            try
            {
                //GetDiscount in DiscountService (discount grpc project) expects a GetDiscountRequest
                var discountRequest = new GetDiscountRequest { ProductName = productName };

                return await _discountProtoService.GetDiscountAsync(discountRequest);
            }
            catch (Exception e)
            {

                Console.Write(e);
                return null;
            }
            
        }
    }
}
