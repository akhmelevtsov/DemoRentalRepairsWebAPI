using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.RentalRepairs.Infrastructure.Repositories.EF
{
    public class PropertiesContext : DbContext
    {
        public PropertiesContext(DbContextOptions<PropertiesContext> options) : base(options)
        {
        }
        public DbSet<PropertyTbl> PropertyTbl { get; set; }
        public DbSet<TenantTbl> TenantTbl { get; set; }
        public DbSet<TenantRequestTbl> TenantRequestTbl { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PropertyTbl>().OwnsOne(p => p.Address);
            modelBuilder.Entity<PropertyTbl>().OwnsOne(p => p.Superintendent);

            modelBuilder.Entity<TenantTbl>().OwnsOne(p => p.ContactInfo);

            //modelBuilder.Entity<TenantRequestTbl>().OwnsOne(p => p.RejectNotes);
            //modelBuilder.Entity<TenantRequestTbl>().OwnsOne(p => p.WorkReport);
            //modelBuilder.Entity<TenantRequestTbl>().OwnsOne(p => p.ServiceWorkOrder); //ServiceWorkOrder.Person
            //modelBuilder.Entity<ServiceWorkOrder>().OwnsOne(p => p.Person);
        }
    }
}
