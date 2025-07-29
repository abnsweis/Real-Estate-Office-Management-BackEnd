using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Sales
{
    public class CreateSaleDTO
    {

        public string? SellerId { get; set; }

        public string? BuyerId { get; set; }

        public string? PropertyId { get; set; }

        public decimal? Price { get; set; }

        [DataType(DataType.Date)]
        public string? SaleDate { get; set; }

        public string? Description { get; set; } 
        public IFormFile? ContractImage { get; set; }  
    }
}
