using AutoMapper;
using RealEstate.Application.Dtos.Users;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Mappings
{ 

    namespace RealEstate.Application.Mappings
    {
        public class UserDtoMappingProfile : Profile
        {
            public UserDtoMappingProfile()
            { 
                CreateMap<UserDomain, UserDTO>()
                    .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id.ToString()))  
                    .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))  
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Person.FullName))
                    .ForMember(dest => dest.NationalID, opt => opt.MapFrom(src => src.Person.NationalId))
                    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth.ToString("yyyy-MM-dd")))  
                    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person.Gender.ToString()))  
                    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Person.ImageURL))
                    .ForMember(dest => dest.phoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
                 
                CreateMap<UserDTO, UserDomain>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.UserID) ? Guid.NewGuid() : Guid.Parse(src.UserID))) 
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)) 
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.phoneNumber)) 
                    .ForMember(dest => dest.Person, opt => opt.MapFrom((src, dest) =>  
                    {
                        
                        var person = dest.Person ?? new Person();  
                        person.FullName = src.FullName;
                        person.NationalId = src.NationalID;
                        person.DateOfBirth = string.IsNullOrEmpty(src.DateOfBirth) ? default : DateOnly.Parse(src.DateOfBirth);  
                        person.Gender = string.IsNullOrEmpty(src.Gender) ? default : (Gender)Enum.Parse(typeof(Gender), src.Gender);  
                        person.ImageURL = src.ImageUrl; 
                        return person;
                    }));
            }
        }
    }
}
