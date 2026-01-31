using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BankMore.ContaCorrente.Application.Commands;

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
            var numeroConta = await _mediator.Send(comando);
            return CreatedAtAction(nameof(CriarConta), new { numeroConta });
        }
    }
}
