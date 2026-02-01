using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Data {
    public class DataBaseContext : DbContext {
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conta>()
                .HasIndex(c => c.NumeroConta)
                .IsUnique();

            modelBuilder.Entity<Movimento>()
                .HasOne(m => m.Conta)
                .WithMany()
                .HasForeignKey(m => m.ContaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transferencia>()
                .HasOne(t => t.ContaOrigem)
                .WithMany()
                .HasForeignKey(t => t.ContaOrigemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transferencia>()
                .HasOne(t => t.ContaDestino) 
                .WithMany()
                .HasForeignKey(t => t.ContaDestinoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movimento>()
                .HasIndex(m => new { m.ContaId, m.RequestId })
                .IsUnique(); 

            modelBuilder.Entity<Transferencia>()
                .HasIndex(t => t.RequestId)
                .IsUnique();  
        }
    }
}