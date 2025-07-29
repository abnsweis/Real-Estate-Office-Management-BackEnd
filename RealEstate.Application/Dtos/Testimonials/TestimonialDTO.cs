using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Testimonials
{
    public class TestimonialDTO
    {
        public string? TestimonialId { get; set; }
        public string? UserId { get; set; }
        public string? RatingText { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? ImageURL { get; set; }
        public string? RatingNumber { get; set; }
    }
}
