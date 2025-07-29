using Microsoft.AspNetCore.Http;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Dtos.Users
{
    public class UpdateUserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!; 
        public string PhoneNumber { get; set; } = default!;
        public enGender Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public IFormFile? ImageURL { get; set; }
    }
}
