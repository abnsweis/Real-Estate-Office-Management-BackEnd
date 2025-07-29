using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Rental
{
    public class RentalDTO
    {
        public string? RentalId { get; set; }
        public string? LessorId { get; set; }
        public string? LessorName { get; set; }
        public string? LessorNationallD { get; set; }

        public string? LesseeId { get; set; }
        public string? LesseeName { get; set; }
        public string? LesseeNationallD { get; set; }

        public string? PropertyId { get; set; }
        public string? PropertyTitle { get; set; }
        public string? PropertyCatagory { get; set; }
        public string? PropertyNumber { get; set; }

        public decimal? RentPriceMonth { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? RentType { get; set; }   
        public string? Duration { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string? Description { get; set; }  

        public string? ContractImageUrl { get; set; }  

    }
}
