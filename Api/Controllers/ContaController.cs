using MediatR;
using Microsoft.AspNetCore.Mvc;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.Validators;
using FluentValidation.Results;
using System;
using BankMore.ContaCorrente.Domain.Exceptions;

namespace BankMore.ContaCorrente.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase {
        private readonly IMediator _mediator;

        public ContaController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaCommand comando) {
            var validador = new CriarContaValidador();
            ValidationResult resultado = await validador.ValidateAsync(comando);

            if (!resultado.IsValid) {
                var erros = string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = erros, failureType = "INVALID_DOCUMENT" });
            }

            try {
                var numeroConta = await _mediator.Send(comando); return Created("", new { numeroConta });
            }
            catch (BusinessException ex) {
                return BadRequest(new { message = ex.Message, failureType = ex.FailureType });
            }
        }
    }
}
