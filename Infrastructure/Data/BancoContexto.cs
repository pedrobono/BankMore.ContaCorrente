using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Data
{
    public class BancoContext : DbContext
    {
        public DbSet<Conta> Contas { get; set; }

        public BancoContext(DbContextOptions<BancoContext> options) : base(options) { }
    }
}
