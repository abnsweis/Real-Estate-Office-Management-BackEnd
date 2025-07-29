using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Data.Interceptors
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _user;
        private readonly TimeProvider _dateTimeProvider;

        public AuditableEntitySaveChangesInterceptor(ICurrentUserService user, TimeProvider dateTimeProvider)
        {
            this._user = user;
            this._dateTimeProvider = dateTimeProvider;
        }


        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
             UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    var utcNow = _dateTimeProvider.GetUtcNow();
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedBy = _user?.UserId ?? Guid.Empty;
                        entry.Entity.CreatedDate = utcNow;
                    }
                    entry.Entity.LastModifiedBy = _user?.UserId ?? Guid.Empty;
                    entry.Entity.LastModifiedDate = utcNow;
                }
            }
        }
    }



    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry)
        {
            return entry.References.Any(
                
                r =>

                r.TargetEntry != null && 
                r.TargetEntry.Metadata.IsOwned() && (
                r.TargetEntry.State == EntityState.Added ||
                r.TargetEntry.State == EntityState.Modified)
                );
        }
    }
    

}
