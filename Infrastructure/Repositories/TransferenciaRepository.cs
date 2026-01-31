using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly DataBaseContext _context;

    public TransferenciaRepository(DataBaseContext context)
    {
        _context = context;
    }

    public async Task Adicionar(Transferencia transferencia)
    {
        await _context.Transferencias.AddAsync(transferencia);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteRequestId(string requestId)
    {
        return await _context.Transferencias.AnyAsync(t => t.RequestId == requestId);
    }
}