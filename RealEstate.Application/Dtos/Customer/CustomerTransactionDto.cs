using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Customer
{
    public class CustomerTransactionDto
    {
        //public Guid TransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public string? TransactionType { get; set; }
        public string? propertyNumber { get; set; }
        public Guid? PropertyId { get; set; }
        //public string? ContractImage { get; set; }
        public decimal? Amount { get; set; }
        public string? TransactionDate { get; set; }
        public string? Notes { get; set; }
    }
}
