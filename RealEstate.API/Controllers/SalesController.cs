using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Application.Features.Sales.Commands;
using RealEstate.Application.Features.Sales.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISender _mediator;
        public SalesController(ISender mediator)
        {
            _mediator = mediator;
        }



        /// <summary>
        /// Retrieves all Sales with pagination.
        /// </summary>
        /// <param name="pagination">Pagination parameters (PageNumber, PageSize)</param> 
        /// <returns>A paginated list of sales.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<SaleDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllSales([FromQuery] PaginationRequest pagination)
        {
            var response = await _mediator.Send(new GetAllSalesQuery(pagination));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }
       
        /// <summary>
        /// Retrieves a single Sale by its unique ID.
        /// </summary>
        /// <param name="SaleId">The ID of the Sale</param>
        /// <returns>The matched Sale if found</returns>
        [HttpGet("Id/{SaleId:guid}")]
        [ProducesResponseType(typeof(SaleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSaleById(Guid SaleId)
        {
            var response = await _mediator.Send(new GetSaleByIdQuery(SaleId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpGet("Property/{PropertyId:guid}")]
        public async Task<IActionResult> GetSalesByPropertyId([FromQuery] PaginationRequest pagination,Guid PropertyId)
        {
            var response = await _mediator.Send(new GetSalesByPropertyIdQuery(pagination,PropertyId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpGet("Seller/{SellerId:guid}")]
        public async Task<IActionResult> GetSalesBySellerId([FromQuery] PaginationRequest pagination,Guid SellerId)
        {
            var response = await _mediator.Send(new GetSalesBySellerIdQuery(pagination,SellerId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }
        [HttpGet("Buyer/{BuyerId:guid}")]
        public async Task<IActionResult> GetSalesByBuyerId([FromQuery] PaginationRequest pagination,Guid BuyerId)
        {
            var response = await _mediator.Send(new GetSalesByBuyerIdQuery(pagination, BuyerId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }


        /// <summary>
        /// Creates a new sale.
        /// </summary>
        /// <param name="saleData">The sale data to create</param>
        /// <returns>The created sale's ID</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromForm] CreateSaleDTO saleData)
        {
            var Command = new CreateSaleCommand(saleData);
            var response = await _mediator.Send(Command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return CreatedAtAction(
                nameof(GetSaleById),
                new { SaleId = response.Data },
                new { SaleId = response.Data });
        }

    }
}
