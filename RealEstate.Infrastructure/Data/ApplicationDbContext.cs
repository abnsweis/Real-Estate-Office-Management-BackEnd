using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data.SeedData;
using RealEstate.Infrastructure.Persistence.Configuration;
using System.Reflection.Emit;

namespace RealEstate.Infrastructure.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid> , IApplicationDbContext
    {
        public DbSet<Person> People { get; set; }


        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Favorite> Favorites { get; set; } 

        public virtual DbSet<Property> Properties { get; set; }

        public virtual DbSet<PropertyImage> PropertyImages { get; set; }

        public virtual DbSet<Rating> Ratings { get; set; }

        public virtual DbSet<Rental> Rentals { get; set; }

        public virtual DbSet<Sale> Sales { get; set; }

        public virtual DbSet<Testimonial> Testimonials { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public ApplicationDbContext() : base(new DbContextOptionsBuilder<ApplicationDbContext>()
        //    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=New_REOMPDb;Trusted_Connection=True;")
        //    .Options)
        //{
        //}




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());

            //IdentityConfiguration.ConfigureIdentity(modelBuilder);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Categori__19093A2BC2002CFA");

                entity.HasIndex(e => e.CategoryName, "UQ_CategoryName").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("CategoryID");
                entity.Property(e => e.CategoryName).HasMaxLength(40);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Comments__C3B4DFAA348EBFAB");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("CommentID");
                entity.Property(e => e.CommentText).HasMaxLength(255);
                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Property).WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comments_PropertyID");

                entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(Comment => Comment.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comments_Users_UserID");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Customer__A4AE64B81FEBED20");

                entity.HasIndex(e => new { e.PersonId, e.CustomerType }, "UQ_Customers_PersonID_CustomerType").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("CustomerID");
                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.HasOne(d => d.Person).WithMany(p => p.Customers)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_Customers_PersonID");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Favorite__CE74FAF5D307196A");

                entity.HasIndex(e => new { e.UserId, e.PropertyId }, "UQ_Favorites_User_Property").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("FavoriteID");
                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Property).WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorites_PropertyID");


                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey( f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Feavorites_Users_UserID");

            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__People__AA2FFB85489A4652");
                 

                entity.HasIndex(e => e.NationalId, "UQ_NationalID").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("PersonID");
                entity.Property(e => e.FullName).HasMaxLength(150);
                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.ImageURL)
                    .HasMaxLength(200)
                    .HasColumnName("ImageURL");
                entity.Property(e => e.NationalId)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NationalID");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Properti__70C9A7556C4F66A3");

                entity.HasIndex(e => e.PropertyNumber, "UQ_PropertyNumber").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("PropertyID");
                entity.Property(e => e.Address).HasMaxLength(150);
                entity.Property(e => e.Area).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(150);
                entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Title).HasMaxLength(100);
                entity.Property(e => e.VideoUrl)
                    .HasMaxLength(200)
                    .HasColumnName("VideoURL");

                entity.HasOne(d => d.Category).WithMany(p => p.Properties)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Categories_CategoryID");

                entity.HasOne(d => d.Owner).WithMany(p => p.Properties)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_Customers_OwnerID");
            });

            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Property__3BCB5D8470DB1133");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("PropertyImageID");
                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(250)
                    .HasColumnName("ImageURL");
                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");

                entity.HasOne(d => d.Property).WithMany(p => p.PropertyImages)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("FK_PropertyImages_PropertyID");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Ratings__FCCDF85CA292D92A");

                entity.HasIndex(e => new { e.UserId, e.PropertyId }, "UQ_Ratings_User_Property").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("RatingID");
                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
                entity.Property(e => e.RatingText).HasMaxLength(255);
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Property).WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ratings_PropertyID");

                entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rating => rating.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Ratings_Users_UserID");
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Rentals__970059636AE92A38");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("RentalID");
                entity.Property(e => e.ContractImageUrl)
                    .HasMaxLength(250)
                    .HasColumnName("ContractImageURL");
                entity.Property(e => e.Description).HasMaxLength(100); 
                entity.Property(e => e.LesseeId).HasColumnName("LesseeID");
                entity.Property(e => e.LessorId).HasColumnName("lessorID");
                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
                entity.Property(e => e.RentPriceMonth).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.StartDate).HasDefaultValueSql("(CONVERT([date],getdate()))");

                entity.HasOne(d => d.Lessee).WithMany(p => p.RentalLessees)
                    .HasForeignKey(d => d.LesseeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rentals_LesseeID");

                entity.HasOne(d => d.Lessor).WithMany(p => p.RentalLessors)
                    .HasForeignKey(d => d.LessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rentals_lessorID");

                entity.HasOne(d => d.Property).WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rentals_PropertyID");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Sales__1EE3C41FC53CEF2A");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("SaleID");
                entity.Property(e => e.BuyerId).HasColumnName("BuyerID");
                entity.Property(e => e.ContractImageUrl)
                    .HasMaxLength(250)
                    .HasColumnName("ContractImageURL");
                entity.Property(e => e.Description).HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
                entity.Property(e => e.SaleDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.SellerId).HasColumnName("SellerID");

                entity.HasOne(d => d.Buyer).WithMany(p => p.SaleBuyers)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sales_BuyerID");

                entity.HasOne(d => d.Property).WithMany(p => p.Sales)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sales_PropertyID");

                entity.HasOne(d => d.Seller).WithMany(p => p.SaleSellers)
                    .HasForeignKey(d => d.SellerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sales_SellerID");
            });

            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Testimon__91A23E5365E364E7"); 
                entity.HasIndex(e => e.UserId, "UQ_Testimonials_UserID").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("TestimonialID");
                entity.Property(e => e.RatingText).HasMaxLength(255);
                entity.Property(e => e.UserId).HasColumnName("UserID");  

                 
                entity.HasOne<ApplicationUser>()  
                    .WithOne()                   
                    .HasForeignKey<Testimonial>(t => t.UserId) 
                    .OnDelete(DeleteBehavior.Cascade)          
                    .HasConstraintName("FK_Testimonials_Users_UserID");  
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
