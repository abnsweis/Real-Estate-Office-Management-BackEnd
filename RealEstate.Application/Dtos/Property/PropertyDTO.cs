using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Property
{
    public class PropertyDTO
    {
        public string PropertyId { get; set; }
        public string Title { get; set; }
        public string OwnerId { get; set; }
        public string CategoryId { get; set; }
        public int PropertyNumber { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string PropertyStatus { get; set; }  
        public decimal Area { get; set; }
        public decimal Rating { get; set; }
        public string? VideoUrl { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public string CategoryName { get; set; } 
        public string OwnerFullName { get; set; }
        public string OwnerNationalId { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}
