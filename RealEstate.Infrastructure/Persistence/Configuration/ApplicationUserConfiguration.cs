using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Persistence.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(u => u.Person)
               .WithOne()
               .HasForeignKey<ApplicationUser>(u => u.personId)
               .OnDelete(DeleteBehavior.Cascade);
             
        }
    }
}
