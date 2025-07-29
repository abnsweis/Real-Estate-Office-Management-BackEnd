using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Ratings
{
    public class CreateUpdateRatingDTO 
    {
        public string? RatingText { get; set; }
        public int? RatingNumber { get; set; }
    }
}
