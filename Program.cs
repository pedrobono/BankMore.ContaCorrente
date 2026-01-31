using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Infrastructure.Data;
using MediatR;
using BankMore.ContaCorrente.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BancoContexto>(options =>
    options.UseSqlite("Data Source=BankMore.db"));

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/contas", async (CriarContaComando comando, IMediator mediator) =>
{
    var resultado = await mediator.Send(comando);
    return Results.Created($"/contas/{resultado}", resultado);
})
.WithName("CriarConta")
.WithOpenApi();

app.Run();