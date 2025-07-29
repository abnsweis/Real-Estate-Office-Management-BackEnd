using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Ratings
{
    public class RatingDTO
    {
        public string? RatinId { get; set; }
        public string? UserId { get; set; }
        public string? PropertyId { get; set; }
        public string? RatingText { get; set; }
        public string? RatingNumber { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? ImageURL { get; set; }
    }
}
