using MediatR;
using Microsoft.AspNetCore.Mvc;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.Validators;
using FluentValidation.Results;
using System;
using BankMore.ContaCorrente.Domain.Exceptions;

namespace BankMore.ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaCommand comando)
        {
            // Validação dos dados com FluentValidation
            var validador = new CriarContaValidador();
            ValidationResult resultado = await validador.ValidateAsync(comando);
            
            if (!resultado.IsValid)
            {
                // Se a validação falhar, retorna um erro com mensagens de validação
                var erros = string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = erros, failureType = "INVALID_DOCUMENT" });
            }

            try
            {
                var numeroConta = await _mediator.Send(comando);
                return CreatedAtAction(nameof(CriarConta), new { numeroConta });
            }
            catch (BusinessException ex)
            {
                // Captura a BusinessException e retorna um erro 400 com a mensagem e tipo de falha
                return BadRequest(new { message = ex.Message, failureType = ex.FailureType });
            }
        }
    }
}
