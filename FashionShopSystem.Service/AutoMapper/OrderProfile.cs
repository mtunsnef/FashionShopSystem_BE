using AutoMapper;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Service.DTOs.OrderDto;

namespace FashionShopSystem.Service.AutoMapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponseDto>()
                            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<OrderDetail, OrderDetailResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));
        }
    }
}
