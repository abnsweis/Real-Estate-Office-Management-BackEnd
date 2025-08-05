using AutoMapper;
using RealEstate.Application.Features.Customers.Commands.Update;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Mappings
{
    public class CustomerProfile : Profile
    {

        public CustomerProfile()
        {

            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Person.FullName))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.Person.NationalId))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Person.ImageURL))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person.Gender.ToString()))
                .ForMember(dest => dest.customerType, opt => opt.MapFrom(src => src.CustomerType.ToString()));
        }
    }
}
