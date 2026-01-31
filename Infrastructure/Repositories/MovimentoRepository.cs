using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly DataBaseContext _context;

    public MovimentoRepository(DataBaseContext context)
    {
        _context = context;
    }

    public async Task Adicionar(Movimento movimento)
    {
        await _context.Movimentos.AddAsync(movimento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteRequestId(string requestId)
    {
        return await _context.Movimentos.AnyAsync(m => m.RequestId == requestId);
    }
}