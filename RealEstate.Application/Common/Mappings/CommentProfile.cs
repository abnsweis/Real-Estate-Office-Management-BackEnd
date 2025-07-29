using AutoMapper;
using RealEstate.Application.Dtos.Comments;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile() {

            CreateMap<Comment, CommentDTO>()
                
                .ForMember(dest => dest.CommentID ,opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.CommentText ,opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.CreatedDate ,opt => opt.MapFrom(src => src.CreatedDate.ToLocalTime().ToString("yyyy-M-d h:mm tt")))
                ;
        
        }
    }
}
