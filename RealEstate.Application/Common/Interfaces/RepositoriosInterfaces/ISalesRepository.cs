using RealEstate.Application.Dtos.Statistics;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface ISalesRepository : IRepository<Sale>
    {

        //List<Sale> GetOwnerProperties(Guid ownerId);
        //List<Sale> GetOwnerPropertiesByNationalId(string nationalId); 
        bool IsSaleExistsById(Guid SaleId);
        decimal GetMonthlySales(int? month = null);
        decimal GetTotalSalesRevenue();
        Task<List<MonthlyFinancialSummaryDTO>> GetMonthlySalesByYearAsync(int year);
    }
}   
