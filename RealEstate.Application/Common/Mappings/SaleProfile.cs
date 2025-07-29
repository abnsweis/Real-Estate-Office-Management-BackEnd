using AutoMapper;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Mappings
{
    public class SaleProfile : Profile
    {

        public SaleProfile() {

            CreateMap<Sale, SaleDTO>()
                .ForMember(dest => dest.SellerId , opt => opt.MapFrom(src => src.SellerId.ToString()))
                .ForMember(dest => dest.SaleId , opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.SellerName , opt => opt.MapFrom(src => src.Seller.Person.FullName))
                .ForMember(dest => dest.SellerPhoneNumber , opt => opt.MapFrom(src => src.Seller.PhoneNumber))
                .ForMember(dest => dest.BuyerName , opt => opt.MapFrom(src => src.Buyer.Person.FullName))
                .ForMember(dest => dest.BuyerId , opt => opt.MapFrom(src => src.BuyerId.ToString()))
                .ForMember(dest => dest.BuyerPhoneNumber , opt => opt.MapFrom(src => src.Buyer.PhoneNumber))
                .ForMember(dest => dest.PropertyId , opt => opt.MapFrom(src => src.PropertyId.ToString()))
                .ForMember(dest => dest.PropertyTitle , opt => opt.MapFrom(src => src.Property.Title))
                .ForMember(dest => dest.PropertyCatagory , opt => opt.MapFrom(src => src.Property.Category.CategoryName))
                .ForMember(dest => dest.PropertyNumber, opt => opt.MapFrom(src => src.Property.PropertyNumber))
                ;
            
        
        }
    }
}
