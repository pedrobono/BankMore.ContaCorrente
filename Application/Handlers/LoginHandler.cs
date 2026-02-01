using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Application.Handlers {
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse> {
        private readonly DataBaseContext _context;
        private readonly IConfiguration _configuration;

        public LoginHandler(DataBaseContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken) {
            var numeroConta = request.CpfOrNumeroConta.Replace("-", "");
            
            var todasContas = await _context.Contas.ToListAsync(cancellationToken);
            
            Conta conta = null;
            
            // Busca por número de conta ou CPF hasheado
            foreach (var c in todasContas) {
                if (c.NumeroConta.Replace("-", "") == numeroConta || BCrypt.Net.BCrypt.Verify(request.CpfOrNumeroConta, c.Cpf)) {
                    conta = c;
                    break;
                }
            }
            
            if (conta == null || !BCrypt.Net.BCrypt.Verify(request.Senha, conta.Senha)) {
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
            }

            var jwtSecretKey = _configuration["JwtSettings:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrEmpty(jwtSecretKey)) {
                throw new InvalidOperationException("A chave secreta do JWT não foi fornecida.");
            }
            // Gerar o token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, conta.Id.ToString()),
                new Claim(ClaimTypes.Name, conta.NumeroConta),
            };

            var token = new JwtSecurityToken(
                issuer: "BankMore",
                audience: "BankMore",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse {
                Token = tokenString
            };
        }
    }
}
