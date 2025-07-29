using AutoMapper;
using RealEstate.Application.Dtos.Testimonials;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Mappings
{
    public class TestimonialProfile : Profile
    {
        public TestimonialProfile()
        {
            CreateMap<Testimonial, TestimonialDTO>()
                .ForMember(t => t.TestimonialId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(t => t.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));
        }
    }
}
