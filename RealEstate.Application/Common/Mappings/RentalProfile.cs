using AutoMapper;
using RealEstate.Application.Dtos.Rental;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Mappings
{
    public class RentalProfile : Profile
    {

        public RentalProfile() {

            CreateMap<Rental, RentalDTO>()
                .ForMember(dest => dest.RentalId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.LessorId, opt => opt.MapFrom(src => src.LessorId.ToString()))
                .ForMember(dest => dest.LessorName, opt => opt.MapFrom(src => src.Lessor.Person.FullName))
                .ForMember(dest => dest.LessorNationallD, opt => opt.MapFrom(src => src.Lessor.Person.NationalId))
                .ForMember(dest => dest.LesseeId, opt => opt.MapFrom(src => src.LesseeId.ToString()))
                .ForMember(dest => dest.LesseeName, opt => opt.MapFrom(src => src.Lessee.Person.FullName))
                .ForMember(dest => dest.LesseeNationallD, opt => opt.MapFrom(src => src.Lessee.Person.NationalId))
                .ForMember(dest => dest.PropertyId , opt => opt.MapFrom(src => src.PropertyId.ToString()))
                .ForMember(dest => dest.PropertyTitle , opt => opt.MapFrom(src => src.Property.Title))
                .ForMember(dest => dest.PropertyCatagory , opt => opt.MapFrom(src => src.Property.Category.CategoryName))
                .ForMember(dest => dest.PropertyNumber, opt => opt.MapFrom(src => src.Property.PropertyNumber))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.GetTotalPrice()))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.GetEndDate()))
                ;
            
        
        }
    }
}
