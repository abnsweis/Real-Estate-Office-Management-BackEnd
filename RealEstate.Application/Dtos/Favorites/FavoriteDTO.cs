using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Favorites
{
    public class FavoriteDTO
    {
        public Guid PropertyId { get; set; }
        public string Title { get; set; }   
        public string MainImage { get; set; }      
        public decimal Price { get; set; }  
        public string Location { get; set; }
    }
}
