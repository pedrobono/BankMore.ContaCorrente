using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BankMore.ContaCorrente.Infrastructure.Repositories
{
    public class ContaRepositorio
    {
        private readonly DataBaseContext _contexto;

        public ContaRepositorio(DataBaseContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<Conta?> ObterContaPorNumero(string numeroConta) => await _contexto.Contas
                .FirstOrDefaultAsync(c => c.NumeroConta == numeroConta);
    }
}
