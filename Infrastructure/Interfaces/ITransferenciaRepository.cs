using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

public interface ITransferenciaRepository
{
    Task Adicionar(Transferencia transferencia);
    Task<bool> ExisteRequestId(string requestId);
}