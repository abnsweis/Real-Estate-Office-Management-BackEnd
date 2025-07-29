using AutoMapper;
using RealEstate.Application.Dtos.Favorites;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Mappings
{
    public class FavoriteProfile : Profile
    {

        public FavoriteProfile()
        {
            CreateMap<Favorite, FavoriteDTO>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Property.Title))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.Property.PropertyImages.FirstOrDefault(img => img.IsMain).ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Property.Price))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Property.Location))
                ;
        }
    }
}
