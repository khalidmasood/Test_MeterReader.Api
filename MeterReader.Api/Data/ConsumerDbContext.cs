using Microsoft.EntityFrameworkCore;
using MeterReader.Api.Models;


namespace MeterReader.Api.Data
{

    public class ConsumerDbContext : DbContext
    {
        public DbSet<ConsumerAccount> ConsumerAccounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        public ConsumerDbContext(DbContextOptions<ConsumerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Defining the primary key and foreign key constraints
            modelBuilder.Entity<MeterReading>()
                .HasIndex(mr => new { mr.AccountId, mr.MeterReadingDateTime })
                .IsUnique();
        }
    }
}
