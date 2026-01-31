using BankMore.ContaCorrente.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.ContaCorrente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimentoController : ControllerBase {
    private readonly IMediator _mediator;

    public MovimentoController(IMediator mediator) {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegistrarMovimento([FromBody] RegistrarMovimentoCommand comando) {
        if (comando == null) {
            return BadRequest(new {
                message = "Os dados do movimento são inválidos.",
                failureType = "INVALID_DATA"
            });
        }

        try {
            await _mediator.Send(comando);
            return NoContent();
        }
        catch (ArgumentException ex) {
            return BadRequest(new {
                message = ex.Message,
                failureType = "INVALID_ACCOUNT"
            });
        }
        catch (Exception ex) {
            return StatusCode(500, new {
                message = "Erro interno ao processar movimento.",
                details = ex.Message
            });
        }
    }
}