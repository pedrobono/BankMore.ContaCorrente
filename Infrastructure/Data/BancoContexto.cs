using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Data
{
    public class BancoContexto : DbContext
    {
        public DbSet<Conta> Contas { get; set; }

        public BancoContexto(DbContextOptions<BancoContexto> options) : base(options) { }
    }
}
