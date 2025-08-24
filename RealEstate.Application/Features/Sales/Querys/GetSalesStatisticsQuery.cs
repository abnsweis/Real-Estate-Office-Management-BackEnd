using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Sales.Querys
{
    public class GetSalesStatisticsQuery : IRequest<AppResponse<SalesStatisticsDto>>
    {
    }

    public class GetSalesStatisticsQueryHandler : IRequestHandler<GetSalesStatisticsQuery, AppResponse<SalesStatisticsDto>>
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IPropertyRepository _propertyRepository;

        public GetSalesStatisticsQueryHandler(ISalesRepository salesRepository,IPropertyRepository propertyRepository)
        {
            this._salesRepository = salesRepository;
            this._propertyRepository = propertyRepository;
        }

        public async Task<AppResponse<SalesStatisticsDto>> Handle(GetSalesStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var totalSales = _salesRepository.GetTotalSalesRevenue();
                var currentMonthSales = _salesRepository.GetMonthlySales();
                var soldCount = await _propertyRepository.GetSoldPropertiesCountAsync();
                var availableCount = await _propertyRepository.GetAvailablePropertiesCountAsync();

                var dto = new SalesStatisticsDto
                {
                    TotalSalesAmount = totalSales,
                    CurrentMonthSalesAmount = currentMonthSales,
                    TotalSoldProperties = soldCount,
                    AvailableProperties = availableCount
                };

                return AppResponse<SalesStatisticsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return AppResponse<SalesStatisticsDto>.Fail(new InternalServerError("server", $"Error fetching sales statistics: {ex.Message}",enApiErrorCode.GeneralError));
            }
        }
    }
}
