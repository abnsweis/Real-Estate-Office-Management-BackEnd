using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Property
{
    public class FilterPropertiesDTO
    {
        public string? Title { get; set; } 
        public string? Location { get; set; } 
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

}
