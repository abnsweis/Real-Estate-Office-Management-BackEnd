using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure.Data;
using System;

namespace RealEstate.Infrastructure.Persistence.Configuration
{
    public static class IdentityConfiguration
    {
        public static void ConfigureIdentity(ModelBuilder builder)
        {
            // ------------------ ApplicationUser ------------------
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
                b.Property(u => u.Id).HasColumnName("UserId");
            });

            // ------------------ IdentityRole ------------------
            builder.Entity<IdentityRole<Guid>>(b =>
            {
                b.ToTable("Roles");
                b.Property(r => r.Id).HasColumnName("RoleId");
            });

            // ------------------ IdentityUserRole ------------------
            builder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.ToTable("UserRoles");
                b.HasKey(ur => new { ur.UserId, ur.RoleId });  
                b.Property(ur => ur.UserId).HasColumnName("UserId");
                b.Property(ur => ur.RoleId).HasColumnName("RoleId");
            });

            // ------------------ IdentityUserClaim ------------------
            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.ToTable("UserClaims");
                b.Property(uc => uc.Id).HasColumnName("UserClaimId");
            });

            // ------------------ IdentityUserLogin ------------------
            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.ToTable("UserLogins");
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });  
                b.Property(l => l.UserId).HasColumnName("UserId");
            });

            // ------------------ IdentityUserToken ------------------
            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.ToTable("UserTokens");
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });  
            });

            // ------------------ IdentityRoleClaim ------------------
            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.ToTable("RoleClaims");
                b.Property(rc => rc.Id).HasColumnName("RoleClaimId");
            });
        }
    }
}
