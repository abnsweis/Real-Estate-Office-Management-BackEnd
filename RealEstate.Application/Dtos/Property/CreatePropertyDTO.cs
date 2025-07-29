using Microsoft.AspNetCore.Http;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Property
{
    public class CreatePropertyDTO 
    {
        public string? Title { get; set; }  

        public string? OwnerId { get; set; } 

        public string? CategoryId { get; set; }  

        public decimal? Price { get; set; }

        public string? Location { get; set; }  

        public string? Address { get; set; }   

        public decimal? Area { get; set; }

        public IFormFile? Video { get; set; }

        public List<IFormFile> Images { get; set; } = new();

        public string? Description { get; set; } 
    }
}
