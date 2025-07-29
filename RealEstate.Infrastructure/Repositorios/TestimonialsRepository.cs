using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Testimonials;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RealEstate.Infrastructure.Repositorios
{
    public class TestimonialsRepository : Repository<Testimonial> , ITestimonialsRepository
    {

        private readonly ApplicationDbContext _context;

        public TestimonialsRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

 
        public async Task<IEnumerable<TestimonialDTO>> GetAllWithUserNamesAsync(int PageNumber,int PageSize)
        {
            var testimonials = (
            from t in _context.Testimonials
            join u in _context.Users on t.UserId equals u.Id
            join p in _context.People on u.personId equals p.Id
            select new TestimonialDTO
            {
                TestimonialId = t.Id.ToString(),
                UserId = t.UserId.ToString(),
                Username = u.UserName,
                FullName = p.FullName,
                RatingText = t.RatingText,
                RatingNumber = t.RatingNumber.ToString(),
                ImageURL = p.ImageURL,
            }
            );
            int skip = (PageNumber - 1) * PageSize;
            testimonials = testimonials.Skip(skip).Take(PageSize);
            return await testimonials.ToListAsync();
        }

        public bool HasSubmittedTestimonial(Guid userId)
        {
            return _context.Testimonials.Any(t => t.UserId == userId);
        }
    }
}
