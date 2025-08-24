using Microsoft.AspNetCore.Http;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Rental
{
        public class CreateRentalDTO
        {   

            public string? LesseeId { get; set; }

            public string? PropertyId { get; set; } 
            public decimal RentPrice { get; set; }
            [DataType(DataType.Date)]
            public DateTime? StartDate { get; set; }
            public RentType RentType { get; set; } = RentType.Monthly;
            public int Duration { get; set; }
            public string? Description { get; set; } 

            public IFormFile? ContractImageUrl { get; set; }  
        }
}
