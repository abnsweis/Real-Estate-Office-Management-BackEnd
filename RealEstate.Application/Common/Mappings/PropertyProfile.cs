using AutoMapper;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Property;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Mappings
{
    public class PropertyProfile  : Profile
    {
        public PropertyProfile()
        {

            CreateMap<Property, PropertyDTO>()
                .ForMember(dest => dest.PropertyId , opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.OwnerId , opt => opt.MapFrom(src => src.OwnerId.ToString()))
                .ForMember(dest => dest.CategoryId , opt => opt.MapFrom(src => src.CategoryId.ToString()))
                .ForMember(dest => dest.PropertyNumber , opt => opt.MapFrom(src => src.PropertyNumber.ToString()))
                .ForMember(dest => dest.MainImage , opt => opt.MapFrom(src => GetMainPropertyImage(src)))
                .ForMember(dest => dest.PropertyStatus , opt => opt.MapFrom(src => src.PropertyStatus.ToString()))
                .ForPath(dest => dest.CategoryName , opt => opt.MapFrom(src => src.Category.CategoryName)) 
                .ForPath(dest => dest.OwnerFullName , opt => opt.MapFrom(src => src.Owner.Person.FullName))
                .ForPath(dest => dest.Rating , opt => opt.MapFrom(src => src.Ratings.Any() ? src.Ratings.Average( r=>r.RatingNumber):0))
                .ForPath(dest => dest.Images , opt => opt.MapFrom(src => src.PropertyImages.Select(
                    
                    i => i.ImageUrl
                    
                    )))
                ;





            CreateMap<CreatePropertyDTO, Property>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                ;


        }
        private string? GetMainPropertyImage(Property property)
        {
            return property.PropertyImages.FirstOrDefault(i => i.IsMain) == null ? null : property.PropertyImages.FirstOrDefault(i => i.IsMain)!.ImageUrl;
        }
    }
}
