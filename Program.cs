using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MediatR;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Application.Commands;
using Microsoft.OpenApi.Any;
using BankMore.ContaCorrente.Api.Middleware;
using BankMore.ContaCorrente.Api.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankMore.ContaCorrente", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // Adicionando os filtros de exemplo personalizados
    c.OperationFilter<SwaggerRequestExampleFilter>();  // Aplicando o filtro de exemplo
    c.OperationFilter<SwaggerResponseOperationFilter>(); // Filtro para as respostas de erro
});

// Configuração do DbContext
builder.Services.AddDbContext<DataBaseContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

// Adicionar MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Configuração do JWT
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("A chave secreta do JWT não foi fornecida.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Usar o middleware de tratamento de erros globalmente
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar HTTPS e autenticação/autorizaçào
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Endpoint de criação de conta (sem autenticação)
app.MapPost("/contas", async (CriarContaCommand comando, IMediator mediator) =>
{
    var resultado = await mediator.Send(comando);
    return Results.Created($"/contas/{resultado}", resultado);
})
.WithName("CriarConta")
.WithOpenApi() // Adicionando o OpenApi para o Swagger gerar a documentação correta
    .Produces<string>(201) // Configurando o retorno com o código 201 e o tipo string
    .Produces<string>(400); // Configurando o retorno para erros de validação, como CPF duplicado

app.Run();
