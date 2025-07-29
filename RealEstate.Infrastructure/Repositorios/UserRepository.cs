using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Data;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.Identity.IdentiyErrors;
using RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RealEstate.Infrastructure.Repositorios
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager ,
            IMapper mapper,
            IFileManager fileManager
            ) {
            this._context = context;
            this._userManager = userManager;
            this._mapper = mapper;
            this._fileManager = fileManager;
        }

        // Add
        public async Task<AppResponse<Guid>> AddAsync(
            UserDomain user,
            CancellationToken cancellationToken = default)
        {
            var applicationUser = _mapper.Map<ApplicationUser>(user);

            var result = await _userManager.CreateAsync(applicationUser, user.Password);
             
            var appResult = result.ToApplicationResult();
             
            if (appResult.IsFailed)
            {
                _fileManager.DeleteFile(applicationUser.Person.ImageURL);
                return AppResponse<Guid>.Fail(appResult.Errors);
            }
             
            var createdUserDomain = _mapper.Map<UserDomain>(applicationUser);
            return AppResponse<Guid>.Success(createdUserDomain.Id);
        }


        // Update
        public async Task<AppResponse> UpdateAsync(UserDomain user)
        {
            var currentUser = _context.Users.Find(user.Id);

            var UpdatedUser = _mapper.Map(user,currentUser);
            var result = await _userManager.UpdateAsync(UpdatedUser);
            var appResult = result.ToApplicationResult();

            if (appResult.IsFailed)
            {
                return AppResponse.Fail(appResult.Errors);
            }
            var createdUserDomain = _mapper.Map<UserDomain>(UpdatedUser);
            return AppResponse.Success(createdUserDomain.Id);
        }
        // GetByIdAsync
        public async Task<UserDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = _context.Users.AsQueryable();
            return await query.Where(user => user.Id == id && user.IsDeleted == false).ProjectTo<UserDomain>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        }
        // Get Count
        public async Task<int> CountAsync(Expression<Func<UserDomain, bool>>? filter = null)
        {
            var query = _context.Users.AsQueryable();

            query = query.AsNoTracking();

            var userDomainQuery = query.ProjectTo<UserDomain>(_mapper.ConfigurationProvider);
            if (filter != null)
            {
                userDomainQuery = userDomainQuery.Where(filter);
            }
            return await userDomainQuery.CountAsync();
        }
        // Get All Async No Filter
        public async Task<IEnumerable<UserDomain>> GetAllAsync(bool asNoTracking = false)
        {
            var query = _context.Users.AsQueryable();
            query = query.AsNoTracking();


            return await query.Where(user => user.IsDeleted == false).ProjectTo<UserDomain>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // Get Where Async
        public async Task<IEnumerable<UserDomain>> GetWhereAsync(Expression<Func<UserDomain, bool>> filter, bool asNoTracking = false)
        {
            var query = _context.Users.AsQueryable();

            query = query.AsNoTracking();
            var userDomainQuery = query.Where(user => user.IsDeleted == false).ProjectTo<UserDomain>(_mapper.ConfigurationProvider);
            if (filter != null)
            {
                userDomainQuery = userDomainQuery.Where(filter);
            }
            return await userDomainQuery.ToListAsync();
        }

        // Get All Async With Filter And Includes And Order
        public async Task<IEnumerable<UserDomain>> GetAllAsync(int pageNumber, int pageSize, Expression<Func<UserDomain, bool>>? filter = null, Func<IQueryable<UserDomain>, IOrderedQueryable<UserDomain>>? orderBy = null, bool asNoTracking = false, params Expression<Func<UserDomain, object>>[] includes)
        {
            IQueryable<ApplicationUser> efQuery = _context.Users.Where(user => user.IsDeleted == false);


            foreach (var include in includes)
            {
                var memberExpression = include.Body as MemberExpression;
                if (memberExpression == null && include.Body is UnaryExpression unaryExpression)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }

                if (memberExpression != null)
                {
                    string propertyName = memberExpression.Member.Name;
                    efQuery = efQuery.Include(propertyName);
                }
            }

            if (asNoTracking)
            {
                efQuery = efQuery.AsNoTracking();
            }


            IQueryable<UserDomain> query = efQuery.ProjectTo<UserDomain>(_mapper.ConfigurationProvider);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy == null)
            {
                query = query.OrderBy(u => u.Id);  
            } else
            {
                query = orderBy(query);
            }
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // جلب النتائج.
            return await query.ToListAsync(); 
        }
         
        public async Task<UserDomain?> FirstOrDefaultAsync(Expression<Func<UserDomain, bool>>? filter = null, Func<IQueryable<UserDomain>, IOrderedQueryable<UserDomain>>? orderBy = null, bool asNoTracking = false, params Expression<Func<UserDomain, object>>[] Includes)
        {
            IQueryable<ApplicationUser> efQuery = _context.Users.Where(user => user.IsDeleted == false); ;
            foreach (var include in Includes)
            {
                var memberExpression = include.Body as MemberExpression;
                if (memberExpression == null && include.Body is UnaryExpression unaryExpression)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                if (memberExpression != null)
                {
                    string propertyName = memberExpression.Member.Name;
                    efQuery = efQuery.Include(propertyName);
                }
            }
             
            if (asNoTracking)
            {
                efQuery = efQuery.AsNoTracking();
            }
             
            IQueryable<UserDomain> query = efQuery.ProjectTo<UserDomain>(_mapper.ConfigurationProvider);
             
            if (filter != null)
            {
                query = query.Where(filter);
            }
             
            if (orderBy != null)
            {
                query = orderBy(query);
            }
             
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }


            

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public bool IsEmailAlreadyTaken(string email)
        {
            string normalizationEmail = _userManager.NormalizeEmail(email);
            return  _userManager.Users.Any(user => user.Email == normalizationEmail && user.IsDeleted == false) ;
        }

        public   bool IsUsernameAlreadyTaken(string username)
        {
            string normalizationUsername = _userManager.NormalizeName(username);
            return _userManager.Users.Any(user => user.UserName == normalizationUsername && user.IsDeleted == false);
        }

        public bool IsPhoneNumberAlreadyTaken(string phoneNumber)
        { 
            return  _userManager.Users.Any(user => user.PhoneNumber == phoneNumber && user.IsDeleted == false);
        }
        

        public async Task<AppResponse> Delete(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == id && user.IsDeleted == false);
            if (user != null) { 
                var result = await _userManager.DeleteAsync(user);
                return new AppResponse { Result = result.ToApplicationResult(), Data = id };
            }

            return new AppResponse { Result = Result.Fail(new NotFoundError("user", "userId", id.ToString(),enApiErrorCode.UserNotFound)), Data = id };
        }

        public bool IsUserExists(Guid id)
        {
            return _userManager.Users.Any(user => user.Id == id && user.IsDeleted == false);
        }
         
    } 
}
