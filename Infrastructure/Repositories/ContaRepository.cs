using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Repositories
{
    public class ContaRepository : IContaRepository 
    {
        private readonly DataBaseContext _context;

        public ContaRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<Conta?> ObterPorNumero(string numeroConta) 
            => await _context.Contas.FirstOrDefaultAsync(c => c.NumeroConta == numeroConta);

        public async Task<Conta?> ObterPorCpf(string cpf) 
            => await _context.Contas.FirstOrDefaultAsync(c => c.Cpf == cpf);

        public async Task Adicionar(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(Conta conta)
        {
            _context.Contas.Update(conta);
            await _context.SaveChangesAsync();
        }
    }
}