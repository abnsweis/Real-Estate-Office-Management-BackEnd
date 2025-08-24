using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Sales
{
    public class SalesStatisticsDto
    {
        public decimal TotalSalesAmount { get; set; }
        public decimal CurrentMonthSalesAmount { get; set; }
        public int TotalSoldProperties { get; set; }
        public int AvailableProperties { get; set; }
    }
}
