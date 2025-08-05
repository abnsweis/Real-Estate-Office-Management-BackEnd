using RealEstate.Application.Dtos.Statistics;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IRentalsRepository : IRepository<Rental>
    {
        bool IsRentalExistsById(Guid RentalId);
        decimal GetMonthlyRental(int? month = null);
        decimal GetTotalRentalsRevenue();
        Task<List<MonthlyFinancialSummaryDTO>> GetMonthlyRentalsByYearAsync(int year);

    }
}

