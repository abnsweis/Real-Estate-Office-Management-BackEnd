using RealEstate.Application.Dtos.Testimonials;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface ITestimonialsRepository : IRepository<Testimonial>
    {
        Task<IEnumerable<TestimonialDTO>> GetAllWithUserNamesAsync(int PageNumber,int PageSize);
        bool HasSubmittedTestimonial(Guid userId);   
    }
}
