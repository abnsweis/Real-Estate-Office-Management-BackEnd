using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Statistics;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositorios
{
    public class SalesRepository : Repository<Sale>, ISalesRepository
    {
        private readonly ApplicationDbContext _context;

        public SalesRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

        public decimal GetMonthlySales(int? month = null)
        {
            var now = DateTime.Now;

            var CurrentMonth = month == null ? now.Month : month;

            var startOfMonth = DateOnly.FromDateTime(new DateTime(now.Year, CurrentMonth.Value, 1));
            var endOfMonth = startOfMonth.AddMonths(1);

            return _context.Sales.Where(s => s.SaleDate >= startOfMonth && s.SaleDate < endOfMonth).Sum(s => s.Price) ?? 0;
        }

        public async Task<List<MonthlyFinancialSummaryDTO>> GetMonthlySalesByYearAsync(int year)
        {
            // Step 1: Fetch sales data grouped by month from DB
            var salesData = await _context.Sales
                .Where(s => s.SaleDate.Year == year)
                .GroupBy(s => s.SaleDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalSales = g.Sum(s => s.Price)
                })
                .ToListAsync();

            // Step 2: Generate months 1..12 and LEFT JOIN with salesData
            var result = Enumerable.Range(1, 12)
                .GroupJoin(
                    salesData,
                    month => month,
                    sale => sale.Month,
                    (month, salesGroup) => new MonthlyFinancialSummaryDTO
                    {
                        Year = year.ToString(),
                        Month = month,
                        MonthName = new System.Globalization.CultureInfo("ar-SY").DateTimeFormat.GetMonthName(month),
                        Total = salesGroup.FirstOrDefault()?.TotalSales ?? 0
                    })
                .ToList();

            return result;
        }

        public async Task<MonthlyFinancialSummaryDTO> GetSalesByMonthAsync(int year, int month)
        {
            var salesInMonth = await _context.Sales
                .Where(s => s.SaleDate.Year == year && s.SaleDate.Month == month)
                .ToListAsync();

            return new MonthlyFinancialSummaryDTO
            {
                Year = year.ToString(),
                Month = month,
                MonthName = new System.Globalization.CultureInfo("ar-SY").DateTimeFormat.GetMonthName(month),
                Total = salesInMonth.Sum(s => s.Price)
            };
        }

        public decimal GetTotalSalesRevenue()
        {
            return _context.Sales.Sum(s => s.Price) ?? 0;
        }
        public bool IsSaleExistsById(Guid SaleId)
        {
            return _context.Sales.Any(p => p.Id == SaleId && !p.IsDeleted);
        }
    }
}
