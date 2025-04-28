using GatCfcDetran.SystemInfra.Entities;
using GatCfcDetran.SystemInfra.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.SystemInfra.DataContext
{
    public class DataContextDb(DbContextOptions<DataContextDb> options) : DbContext(options)
    {
        public override int SaveChanges()
        {
            UpdateLastUpdateDates();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateLastUpdateDates();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateLastUpdateDates()
        {
            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => e.Entity is Entity && e.State == EntityState.Modified))
            {
                ((Entity)entry.Entity).LastUpdateDate = DateTime.UtcNow;
            }
        }

        public DbSet<ScheduleEntity> Schedules { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserProgressEntity> UsersProgress { get; set; }
        public DbSet<CfcEntity> Cfcs { get; set; }
    }
}
