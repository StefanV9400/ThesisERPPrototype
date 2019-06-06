using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VERPS.WebApp.Database.Models;

namespace VERPS.WebApp.Database
{
    public class VERPSDBContext : IdentityDbContext<User>
    {

        public VERPSDBContext()
        {

        }

        //public DbSet<Provider> Providers { get; set; }

        // Exact
        public DbSet<ExactConfiguration> ExactConfigurations { get; set; }
        public DbSet<ExactSupplierConfig> ExactSupplierConfigurations { get; set; }
        public DbSet<ExactOrder> ExactOrders { get; set; }
        public DbSet<ExactOrderLine> ExactOrderLines { get; set; }
        public DbSet<ExactItem> ExactItems { get; set; }
        public DbSet<ExactToken> ExactTokens { get; set; }
        public DbSet<ExactSupplier> ExactSuppliers { get; set; }

        public VERPSDBContext(DbContextOptions<VERPSDBContext> options)
           : base(options)
        {
        }

        public object Include()
        {
            throw new NotImplementedException();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           if (!optionsBuilder.IsConfigured)
           {
               optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=Test1234;Database=INDI_APP;");
           }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");
           base.OnModelCreating(modelBuilder);
        }

    }
}
