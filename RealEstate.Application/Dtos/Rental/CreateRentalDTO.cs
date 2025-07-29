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
        public string? LessorId { get; set; }

        public string? LesseeId { get; set; }

        public string? PropertyId { get; set; } 
        public decimal RentPrice { get; set; }
        [DataType(DataType.Date)]
        public string? StartDate { get; set; }
        [DataType(DataType.Date)]
        public enRentType RentType { get; set; } = enRentType.Monthly;
        public int Duration { get; set; }
        public string? Description { get; set; } = null!;

        public IFormFile? ContractImageUrl { get; set; } = null!;
    }
}
