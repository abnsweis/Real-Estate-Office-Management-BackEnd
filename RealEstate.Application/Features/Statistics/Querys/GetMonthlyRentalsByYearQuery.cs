using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Statistics.Querys
{
    public class GetMonthlyRentalsByYearQuery : IRequest<AppResponse<List<MonthlyFinancialSummaryDTO>>>
    {
        public int Year { get; set; }

        public GetMonthlyRentalsByYearQuery(int year)
        {
            Year = year;
        }
    }

    public class GetMonthlyRentalsByYearQueryHandler : IRequestHandler<GetMonthlyRentalsByYearQuery, AppResponse<List<MonthlyFinancialSummaryDTO>>>
    { 
        private readonly IRentalsRepository _rentalsRepository;  

        public GetMonthlyRentalsByYearQueryHandler(IRentalsRepository rentalsRepository)
        { 
            this._rentalsRepository = rentalsRepository;  
        }

        public async Task<AppResponse<List<MonthlyFinancialSummaryDTO>>> Handle(GetMonthlyRentalsByYearQuery request, CancellationToken cancellationToken)
        { 

            var results = await _rentalsRepository.GetMonthlyRentalsByYearAsync(request.Year);

   
            return AppResponse<List<MonthlyFinancialSummaryDTO>>.Success(results);
        }


         

    }
}
