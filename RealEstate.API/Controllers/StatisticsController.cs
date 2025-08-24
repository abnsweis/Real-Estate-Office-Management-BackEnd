using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Features.Statistics.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ISender _mediator;


        public StatisticsController(ISender mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories(){
            var response = await _mediator.Send(new GetStatisticsQuery());
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpGet("monthly-sales")]
        public async Task<IActionResult> GetMonthlySalesByYear([FromQuery] int year)
        {
            var response = await _mediator.Send(new GetMonthlySalesByYearQuery(year));
            if (response.Result.IsFailed)
                return response.Result.ToActionResult();

            return Ok(response.Data);
        }

        [HttpGet("monthly-sales/by-month")]
        public async Task<IActionResult> GetSalesByMonth([FromQuery] int year, [FromQuery] int month)
        {
            var response = await _mediator.Send(new GetSalesByMonthQuery(year, month));

            if (response.Result.IsFailed)
                return response.Result.ToActionResult();

            return Ok(response.Data);
        }

        [HttpGet("monthly-rentals")]
        public async Task<IActionResult> GetMonthlyRentalsByYear([FromQuery] int year)
        {
            var response = await _mediator.Send(new GetMonthlyRentalsByYearQuery(year));
            if (response.Result.IsFailed)
                return response.Result.ToActionResult();

            return Ok(response.Data);
        }
        [HttpGet("total-sales-revenue")]
        public async Task<IActionResult> GetTotalSalesRevenue()
        {
            var response = await _mediator.Send(new GetTotalSalesRevenueQuery());

            if (response.Result.IsFailed)
                return response.Result.ToActionResult();

            return Ok(new { TotalSalesRevenue = response.Data });
        }
        [HttpGet("total-rentals-revenue")]
        public async Task<IActionResult> GetTotalRentalsRevenue()
        {
            var response = await _mediator.Send(new GetTotalRentalsRevenueQuery());

            if (response.Result.IsFailed)
                return response.Result.ToActionResult();

            return Ok(new { TotalRentalsRevenue = response.Data });
        }
    }

}


