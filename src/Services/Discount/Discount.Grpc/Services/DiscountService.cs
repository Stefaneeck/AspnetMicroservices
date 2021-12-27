using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    //must extend class which has been generated from the protobuf file
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        //more or less the controller class in web api project

        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        //override protobuf methods to expose the grpc methods
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if(coupon == null)
            {
                //only productName is available on this request because that's how the GetDiscountRequest has been defined in the proto file
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with productname={request.ProductName} has not been found."));
            }

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            //CreateDiscountRequest works with CouponModel instead of Coupon, but the repository method expects Coupon
            //map CouponModel from request to Coupon
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.CreateDiscount(coupon);

            _logger.LogInformation("Discount has been created successfully. ProductName: {ProductName}", coupon.ProductName);
            var couponModel = _mapper.Map<CouponModel>(coupon);
            
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.UpdateDiscount(coupon);

            _logger.LogInformation("Discount has been updated successfully. ProductName: {ProductName}", coupon.ProductName);
            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);

            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }
    }
}
