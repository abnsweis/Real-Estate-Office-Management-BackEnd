using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.Users;
using RealEstate.Application.Features.Users.Commands.Create;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public IdentityService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this._context = context;
            this._userManager = userManager;
            this._mapper = mapper;
        }

        public async Task<Result<Guid>> CreateUserAsync(CreateUserCommand registerCommand, CancellationToken cancellationToken)
        {
             using var transaction = await _context.Database.BeginTransactionAsync();

            var person = new Person
            {
                FullName = registerCommand.Data.FullName, 
                ImageURL ="ssssssss", 
                Gender = registerCommand.Data.Gender,
                DateOfBirth = registerCommand.Data.DateOfBirth.Value
            };


            await _context.People.AddAsync(person);
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Saved Person: {person.Id}");


            if (person.Id == Guid.Empty)
            {
                await transaction.RollbackAsync();
                return Result.Fail(new Error("Field To Create User"));
            }

            var user = new ApplicationUser
            {

                Email = registerCommand.Data.Email,
                UserName = registerCommand.Data.Username,
                personId = person.Id

            };


           var results = await _userManager.CreateAsync(user,registerCommand.Data.Password);

            if (!results.Succeeded)
            {
                await transaction.RollbackAsync();
                return results.ToApplicationResult();

            }

            await transaction.CommitAsync();

             
            return Result.Ok(user.Id);
        }

        public Task<bool> AuthorizeAsync(Guid userId, string policyName)
        {
            throw new NotImplementedException();
        }


        public async Task<Result> DeleteUserAsync(Guid userId)
        {

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user is not null)
            {
                user.IsDeleted = true;
                return Result.Ok();   
            }
            return Result.Fail(new NotFoundError("user", "Id", userId.ToString(), enApiErrorCode.UserNotFound));
        }

        public async Task<Result<string?>> GetUserNameAsync(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user is not null)
            { 
                return user.UserName;
            }
            return Result.Fail(new NotFoundError("user", "Id", userId.ToString(), enApiErrorCode.UserNotFound));
        }

        public Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _userManager.Users.Select(user => new UserDTO {
            
                UserID = user.Id.ToString(),
                Username = user.UserName,
                phoneNumber = user.PhoneNumber,
                Email = user.Email,
                FullName = user.Person.FullName,
                NationalID = user.Person.NationalId,
                DateOfBirth = user.Person.DateOfBirth.ToString(),
                Gender = user.Person.Gender.ToString(),
                ImageUrl = user.Person.ImageURL,

            }).ToListAsync();
        }

        public async Task<UserDTO?> GetUserIdAsync(Guid userId)
        {
            var user = await _context.Users.Include(user => user.Person).FirstOrDefaultAsync(u => u.Id == userId);

             
            return _mapper.Map<UserDTO>(user);

        }
    }
}
