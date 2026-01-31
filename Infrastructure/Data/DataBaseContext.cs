using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Data
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Conta> Contas { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conta>()
                .HasIndex(c => c.NumeroConta)
                .IsUnique();
        }
    }
}
