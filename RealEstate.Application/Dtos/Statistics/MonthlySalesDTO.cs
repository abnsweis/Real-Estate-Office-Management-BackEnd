using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Statistics
{
    public class MonthlyFinancialSummaryDTO
    {
        public int  Month { get; set; }
        public string? MonthName { get; set; }
        public decimal? Total { get; set; } 
    }
}
