using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Data.SeedData
{
    public class SeedingData
    {
        private readonly IServiceProvider serviceProvider;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly ApplicationDbContext _context; 
        private readonly IFileManager _fileManager; 
        public SeedingData(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            _fileManager = serviceProvider.GetRequiredService<IFileManager>();
        }





        private async Task AddPeopleDataAsync()
        {  
            var result1 = _fileManager.SetDefaultUserProfileImage()  ;
            var result2 = _fileManager.SetDefaultUserProfileImage()  ;
            Person person1 = new Person { 
                FullName = "ابراهيم الصويص",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(2003, 3, 1)),
                Gender = enGender.Male,
                ImageURL = result1.Value,
            };
            Person person2 = new Person
            {
                FullName = "محمد الصويص",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(2003, 3, 1)),
                Gender = enGender.Male,
                ImageURL = result2.Value,
            };

            await _context.People.AddRangeAsync(person1,person2);

            await _context.SaveChangesAsync();
        }

        private async Task AddUsersDataAsync()
        {

            if (!_context.People.Any()) return;
            

            
            var person1 = _context.People.FirstOrDefault(p => p.FullName == "ابراهيم الصويص");
            var person2 = _context.People.FirstOrDefault(p => p.FullName == "محمد الصويص");


            var user1 = new ApplicationUser
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                personId = person1.Id,
                UserName = "sweis",
                Email = "sweis@gmail.com",
                PhoneNumber = "+12345678911"
            };

            var user2= new ApplicationUser
            {
                Id = Guid.Parse("22222222-2222-2222-1111-222222222222"),
                personId = person2.Id,
                UserName = "asmail",
                Email = "asmail@gmail.com",
                PhoneNumber = "+12345678922"
            };

              
            var resultCreateUser1 = await _userManager.CreateAsync(user1,"@Sweis2003");
            var resultCreateUser2 = await _userManager.CreateAsync(user2,"@Sweis2003");
            var superAdminRole =  _roleManager.Roles.FirstOrDefault(r => r.Name == "SuperAdmin");
            if (resultCreateUser1.Succeeded)
            {
                await _userManager.AddToRoleAsync(user1, superAdminRole?.Name);
            }
            if (resultCreateUser2.Succeeded)
            { 
                await _userManager.AddToRoleAsync(user2, superAdminRole?.Name);
            }

        }

        private async Task AddRolesDataAsync()
        {
            var roles = new List<string> { "User", "Admin", "SuperAdmin" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = roleName, ConcurrencyStamp = Guid.NewGuid().ToString() });
                }
            }
        }

        private async Task AddCustomersDataAsync()
        { 
            if (!_context.People.Any()) return;
             
            var person1 = _context.People.FirstOrDefault(p => p.FullName == "ابراهيم الصويص");
            var person2 = _context.People.FirstOrDefault(p => p.FullName == "محمد الصويص");

 
            if (person1 == null || person2 == null) return;
             
            bool customerExists1 = _context.Customers.Any(c => c.PersonId == person1.Id);
            bool customerExists2 = _context.Customers.Any(c => c.PersonId == person2.Id);
            person1.NationalId = "021515516";
            person2.NationalId = "021515517";
            if (!customerExists1)
            {
                var customer1 = new Customer
                {
                    PersonId = person1.Id,
                    CustomerType = enCustomerType.Buyer,
                    PhoneNumber = "+905375699970",
                     
                };

                _context.Customers.Add(customer1);
            }

            if (!customerExists2)
            {
                var customer2 = new Customer
                {
                    PersonId = person2.Id,
                    CustomerType = enCustomerType.Owner,
                    PhoneNumber = "+905375699971"
                };

                _context.Customers.Add(customer2);
            }

            await _context.SaveChangesAsync();
        }


        private async Task AddCategoriesDataAsync()
        {
            // If categories already exist, don't add duplicates
            if (_context.Categories.Any()) return;

            var categories = new List<Category>
            {
                new Category { CategoryName = "Apartments" },
                new Category { CategoryName = "Villas" },
                new Category { CategoryName = "Commercial Shops" },
                new Category { CategoryName = "Land Plots" }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        private async Task AddPropertiesDataAsync()
        {
            if (!_context.Categories.Any() || !_context.Customers.Any()) return;

            var owner1 = _context.Customers.FirstOrDefault(c => c.Person.NationalId == "021515516");
            var owner2 = _context.Customers.FirstOrDefault(c => c.Person.NationalId == "021515517"); 
            if (owner1 == null || owner2 == null) return;
             
            var category = _context.Categories.FirstOrDefault( c => c.CategoryName == "Land Plots");
            var category2 = _context.Categories.FirstOrDefault( c => c.CategoryName == "Villas");
            if (category == null || category2 == null) return;

            var properties = new List<Property>
            {                new Property
                {
                    PropertyNumber = "1222",
                    Title = "شقة فاخرة في وسط المدينة",
                    OwnerId = owner1.Id,
                    CategoryId = category.Id,
                    Price = 100m,
                    Location = "دمشق - المزة",
                    Address = "شارع المزة 10",
                    PropertyStatus = enPropertyStatus.Available,
                    Area = 120.5m,
                    VideoUrl = null,
                    Description = "شقة حديثة مع إطلالة جميلة ومساحة واسعة"
                },
                new Property
                {
                    PropertyNumber = "1102",
                    Title = "منزل ريفي في ضواحي حلب",
                    OwnerId = owner1.Id,
                    CategoryId = category.Id,
                    Price = 55,
                    Location = "حلب - الريف الغربي",
                    Address = "قرية الكفر",
                    PropertyStatus = enPropertyStatus.Sold,
                    Area = 200.0m,
                    VideoUrl = null,
                    Description = "منزل ريفي مريح، تحيط به الطبيعة، مناسب للعائلات"
                },                new Property
                {
                    PropertyNumber = "1201",
                    Title = "شقة فاخرة في وسط المدينة",
                    OwnerId = owner1.Id,
                    CategoryId = category.Id,
                    Price = 100m,
                    Location = "دمشق - المزة",
                    Address = "شارع المزة 10",
                    PropertyStatus = enPropertyStatus.Available,
                    Area = 120.5m,
                    VideoUrl = null,
                    Description = "شقة حديثة مع إطلالة جميلة ومساحة واسعة"
                },
                new Property
                {
                    PropertyNumber = "1022",
                    Title = "منزل ريفي في ضواحي حلب",
                    OwnerId = owner1.Id,
                    CategoryId = category.Id,
                    Price = 55,
                    Location = "حلب - الريف الغربي",
                    Address = "قرية الكفر",
                    PropertyStatus = enPropertyStatus.Sold,
                    Area = 200.0m,
                    VideoUrl = null,
                    Description = "منزل ريفي مريح، تحيط به الطبيعة، مناسب للعائلات"
                },
                new Property
                {
                    PropertyNumber = "1001",
                    Title = "شقة فاخرة في وسط المدينة",
                    OwnerId = owner1.Id,
                    CategoryId = category.Id,
                    Price = 100m,
                    Location = "دمشق - المزة",
                    Address = "شارع المزة 10",
                    PropertyStatus = enPropertyStatus.Available,
                    Area = 120.5m,
                    VideoUrl = null,
                    Description = "شقة حديثة مع إطلالة جميلة ومساحة واسعة"
                },
                new Property
                {
                    PropertyNumber = "1002",
                    Title = "منزل ريفي في ضواحي حلب",
                    OwnerId = owner1.Id,
                    CategoryId = category.Id,
                    Price = 55,
                    Location = "حلب - الريف الغربي",
                    Address = "قرية الكفر",
                    PropertyStatus = enPropertyStatus.Sold,
                    Area = 200.0m,
                    VideoUrl = null,
                    Description = "منزل ريفي مريح، تحيط به الطبيعة، مناسب للعائلات"
                },
                new Property
                {
                    PropertyNumber = "1003",
                    Title = "محل تجاري على شارع رئيسي",
                    OwnerId = owner2.Id,
                    CategoryId = category2.Id,
                    Price = 200,
                    Location = "حلب - شارع الحضارة",
                    Address = "مقابل البنك المركزي",
                    PropertyStatus = enPropertyStatus.Rented,
                    Area = 80.0m,
                    VideoUrl = null,
                    Description = "محل تجاري بموقع حيوي، مناسب لجميع أنواع الأعمال"
                }
            };
             
            foreach (var property in properties)
            {
                bool exists = _context.Properties.Any(p => p.PropertyNumber == property.PropertyNumber);
                if (!exists)
                {
                    _context.Properties.Add(property);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task AddTestimonialsDataAsync()
        {
            // Check if any testimonials already exist
            if (_context.Testimonials.Any()) return;

            // Get some existing users to associate testimonials with
            var user1 = _context.Users.FirstOrDefault(u => u.UserName == "sweis");
            var user2 = _context.Users.FirstOrDefault(u => u.UserName == "asmail");

            if (user1 == null || user2 == null) return;

            var testimonials = new List<Testimonial>
            {
                new Testimonial
                {
                    Id = Guid.NewGuid(),
                    UserId = user1.Id,
                    RatingText = "Excellent service and very responsive!",
                    RatingNumber = 5
                },
                new Testimonial
                {
                    Id = Guid.NewGuid(),
                    UserId = user2.Id,
                    RatingText = "Good support but can improve response time.",
                    RatingNumber = 4
                }
            };

            _context.Testimonials.AddRange(testimonials);
            await _context.SaveChangesAsync();
        }
        private async Task AddRatingsDataAsync()
        {
            // Avoid duplicate seeding
            if (_context.Ratings.Any()) return;

            // Get a user and a property to link ratings
            var user1 = _context.Users.FirstOrDefault(u => u.UserName == "sweis");
            var user2 = _context.Users.FirstOrDefault(u => u.UserName == "asmail");
            var property = _context.Properties.FirstOrDefault();

            if (user1 == null || user2 == null || property == null) return;

            var ratings = new List<Rating>
            {
                new Rating
                {
                    UserId = user1.Id,
                    PropertyId = property.Id,
                    RatingNumber = 5,
                    RatingText = "ممتاز جدا، أنصح به بشدة!"   
                },
                new Rating
                {
                    UserId = user2.Id,
                    PropertyId = property.Id,
                    RatingNumber = 4,
                    RatingText = "الموقع جيد لكن يحتاج بعض التحديثات."
                }
            };

            _context.Ratings.AddRange(ratings);
            await _context.SaveChangesAsync();
        }

        private async Task AddPropertyImagesDataAsync()
        { 
            if (_context.PropertyImages.Any()) return;
             
            var property = _context.Properties.FirstOrDefault();
            if (property == null) return;

            var images = new List<PropertyImage>();

            foreach (var item in _context.Properties)
            {
                images.Add(new PropertyImage
                {
                    PropertyId = item.Id,
                    ImageUrl = _fileManager.SetDefaultUserProfileImage().Value,
                    IsMain = true
                });
                
            }

            _context.PropertyImages.AddRange(images);
            await _context.SaveChangesAsync();
        }
        private async Task AddCommentsDataAsync()
        {
            if (_context.Comments.Any()) return;

            var user = _context.Users.FirstOrDefault();
            var property = _context.Properties.FirstOrDefault();

            if (user == null || property == null) return;

            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PropertyId = property.Id,
                    CommentText = "العقار مرتب ونظيف، بس السعر شوي غالي.",
                    CreatedDate = DateTimeOffset.UtcNow
                },
                new Comment
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PropertyId = property.Id,
                    CommentText = "الموقع ممتاز وفيه خدمات حواليه.",
                    CreatedDate = DateTimeOffset.UtcNow
                }
            };

            _context.Comments.AddRange(comments);
            await _context.SaveChangesAsync();
        }
        private async Task AddFavoritesDataAsync()
        {
            if (_context.Favorites.Any()) return;

            var user = _context.Users.FirstOrDefault();
            var property = _context.Properties.FirstOrDefault();

            if (user == null || property == null) return;

            var favorite = new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PropertyId = property.Id,
                CreatedDate = DateTimeOffset.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }
        private async Task AddRentalsDataAsync()
        {
            if (_context.Rentals.Any()) return;

            var lessor = _context.Customers.FirstOrDefault();  
            var lessee = _context.Customers.Skip(1).FirstOrDefault();  
            var property = _context.Properties.FirstOrDefault();

            if (lessor == null || lessee == null || property == null) return;

            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                LessorId = lessor.Id,
                LesseeId = lessee.Id,
                PropertyId = property.Id,
                RentPriceMonth = 1500.00m,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow), 
                Duration = 3,
                RentType = enRentType.Monthly,
                Description = "عقد إيجار سنوي لعقار مميز",
                ContractImageUrl = "/Uploads/Contracts/sample-rental-contract.jpg"
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
        }
        private async Task AddSalesDataAsync()
        {
            if (_context.Sales.Any()) return;

            var seller = _context.Customers.FirstOrDefault();  
            var buyer = _context.Customers.Skip(1).FirstOrDefault(); 
            var property = _context.Properties.FirstOrDefault( p => p.PropertyStatus == enPropertyStatus.Sold);
            var result =  _fileManager.SetDefaultContractImage();
            if (seller == null || buyer == null || property == null) return;

            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SellerId = seller.Id,
                BuyerId = buyer.Id,
                PropertyId = property.Id,
                Price = 200000.00m,
                SaleDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Description = "بيع عقار سكني في موقع ممتاز",
                ContractImageUrl =  result.Value 
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
        }

        public async Task AddSeddingData()
        {
            await AddPeopleDataAsync();
            await AddRolesDataAsync();
            await AddUsersDataAsync();
            await AddCustomersDataAsync();
            await AddCategoriesDataAsync();
            await AddPropertiesDataAsync();
            await AddTestimonialsDataAsync();
            await AddRatingsDataAsync();
            await AddPropertyImagesDataAsync();
            await AddCommentsDataAsync();
            await AddFavoritesDataAsync();
            await AddRentalsDataAsync();
            await AddSalesDataAsync();
        }
    }
}
