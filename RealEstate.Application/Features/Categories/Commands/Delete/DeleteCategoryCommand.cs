using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Commands.Delete
{
    public record DeleteCategoryCommand(Guid categoryId) : IRequest<AppResponse>;

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, AppResponse>
    { 
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<AppResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {


            var category = await _categoryRepository.FirstOrDefaultAsync(category => category.Id == request.categoryId && category.IsDeleted == false);
            if (category is null)
            {

                var result = Result.Fail(new NotFoundError(
                    "category",
                    "categoryId",
                    request.categoryId.ToString(),
                    enApiErrorCode.CategoryNotFound));

                return new AppResponse { Result = result, Data = request.categoryId };
            }

            _categoryRepository.Delete(category);

            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return new AppResponse
            {
                Result = Result.Ok()
            };
        }

    }

}
