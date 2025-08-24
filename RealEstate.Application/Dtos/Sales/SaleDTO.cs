using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Sales
{
    public class SaleDTO
    {
        public string SaleId { get; set; }
        public string SellerId { get; set; }

        public string BuyerId { get; set; }

        public string PropertyId { get; set; }

        public decimal? Price { get; set; }

        public string SaleDate { get; set; }

        public string Description { get; set; } = null!;

        public string ContractImageUrl { get; set; } = null!;
        public string SellerName { get; set; } = null!;
        public string SellerPhoneNumber { get; set; } = null!;
        public string BuyerName { get; set; } = null!;
        public string BuyerPhoneNumber { get; set; } = null!;
        public string PropertyTitle { get; set; } = null!;
        public string PropertyCatagory { get; set; } = null!;
        public string PropertyNumber { get; set; } = null!;

    }
}
