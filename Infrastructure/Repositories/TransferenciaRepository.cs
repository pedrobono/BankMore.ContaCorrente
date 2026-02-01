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
        // Transferencia não está no schema atual do banco
        await Task.CompletedTask;
    }

    public async Task<bool> ExisteRequestId(string requestId)
    {
        // Transferencia não está no schema atual do banco
        return await Task.FromResult(false);
    }
}