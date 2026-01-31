using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

public interface IMovimentoRepository
{
    Task Adicionar(Movimento movimento);
    Task<bool> ExisteRequestId(string requestId);
}