using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Statistics;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositorios
{
    public class RentalsRepository : Repository<Rental>, IRentalsRepository
    {
        private readonly ApplicationDbContext _context;
        
        public RentalsRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }
        public decimal GetMonthlyRental(int? month = null)
        {
            var now = DateTimeOffset.Now;  

            var currentMonth = month ?? now.Month;
            var startOfMonth = new DateTimeOffset(now.Year, currentMonth, 1, 0, 0, 0, now.Offset);
            var endOfMonth = startOfMonth.AddMonths(1);

            return _context.Rentals
                .Where(r => r.CreatedDate >= startOfMonth && r.CreatedDate < endOfMonth).ToList()
                .Sum(r => r.GetTotalPrice());  
        }

        public async Task<List<MonthlyFinancialSummaryDTO>> GetMonthlyRentalsByYearAsync(int year)
        {
            // Step 1: Fetch rentals from DB for the given year
            var rentals = await _context.Rentals
                .Where(s => s.CreatedDate.Year == year)
                .ToListAsync();

            // Step 2: Group rentals by month and calculate total
            var groupedData = rentals
                .GroupBy(s => s.CreatedDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalSales = g.Sum(s =>
                    {
                        int totalMonths = s.RentType == enRentType.Yearly ? s.Duration * 12 : s.Duration;
                        return totalMonths * s.RentPriceMonth;
                    })
                })
                .ToList();

            // Step 3: Generate months 1..12 and LEFT JOIN with groupedData
            var result = Enumerable.Range(1, 12)
                .GroupJoin(
                    groupedData,
                    month => month,
                    data => data.Month,
                    (month, salesGroup) => new MonthlyFinancialSummaryDTO
                    {
                        Month = month,
                        MonthName = new System.Globalization.CultureInfo("ar-SY").DateTimeFormat.GetMonthName(month),
                        Total = salesGroup.FirstOrDefault()?.TotalSales ?? 0
                    })
                .ToList();

            return result;
        }

        public decimal GetTotalRentalsRevenue()
        {
            return _context.Rentals.Sum(s =>
                (s.RentType == enRentType.Yearly ? s.Duration * 12 : s.Duration) * s.RentPriceMonth
            ) ;
        }

        public bool IsRentalExistsById(Guid RentalId)
        {
            return _context.Rentals.Any(p => p.Id == RentalId && !p.IsDeleted);
        }
    }
}
