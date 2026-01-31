using BankMore.ContaCorrente.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SaldoController : ControllerBase {
        private readonly IMediator _mediator;

        public SaldoController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(SaldoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterSaldo() {
            var contaId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(contaId)) {
                return Unauthorized(new { message = "Token inválido ou expirado", failureType = "USER_UNAUTHORIZED" });
            }

            var saldo = await _mediator.Send(new ObterSaldoQuery { ContaId = Guid.Parse(contaId) });

            return Ok(saldo);
        }
    }
}
