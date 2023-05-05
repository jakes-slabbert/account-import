using Bitventure_Accounts.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bitventure_Accounts.Server.Context
{
    public class AccountsDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AccountsDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server with connection string from app settings
            options.UseSqlServer(Configuration.GetConnectionString("AzDb"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Master>()
                .HasIndex(u => u.AccountNumber)
                .IsUnique();

            builder.Entity<Detail>()
                .HasIndex(detail => detail.PaymentId)
                .IsUnique();

            builder.Entity<Detail>()
                .HasOne(detail => detail.Account)
                .WithMany(account => account.Details)
                .HasForeignKey("AccountId");
        }

        public DbSet<Master> Masters { get; set; }
        public DbSet<Detail> Details { get; set; }
    }
}
