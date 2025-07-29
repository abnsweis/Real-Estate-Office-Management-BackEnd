using AutoMapper;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Mappings
{
    public class RatingProfile : Profile
    {

        public RatingProfile()
        {
            CreateMap<Rating,RatingDTO>()
                .ForMember(dest => dest.RatinId,opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.PropertyId, opt => opt.MapFrom(src => src.PropertyId.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.RatingText, opt => opt.MapFrom(src => src.RatingText))
                .ForMember(dest => dest.RatingNumber, opt => opt.MapFrom(src => src.RatingNumber.ToString()))
                ;
        }
    }
}
