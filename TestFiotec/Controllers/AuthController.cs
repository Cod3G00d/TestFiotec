using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestFiotec.DTOs;
using TestFiotec.Services.Interface;

namespace TestFiotec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISolicitanteService _solicitanteService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration configuration,
            ISolicitanteService solicitanteService,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _solicitanteService = solicitanteService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.CPF))
                {
                    return BadRequest(new { mensagem = "CPF é obrigatório" });
                }

                // Remover formatação do CPF (pontos e traços)
                string cpfLimpo = new string(loginDto.CPF.Where(char.IsDigit).ToArray());

                // Validar o formato do CPF (deve ter 11 dígitos)
                if (cpfLimpo.Length != 11)
                {
                    return BadRequest(new { mensagem = "CPF inválido" });
                }

                // Buscar o solicitante pelo CPF
                var solicitante = await _solicitanteService.GetByCPFAsync(cpfLimpo);

                if (solicitante == null)
                {
                    return Unauthorized(new { mensagem = "CPF não encontrado" });
                }

                // Gerar token JWT
                var token = GerarToken(solicitante);

                return Ok(new
                {
                    token,
                    solicitante = new
                    {
                        id = solicitante.Id,
                        nome = solicitante.Nome,
                        cpf = solicitante.CPF
                    },
                    mensagem = "Login realizado com sucesso"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o login");
                return StatusCode(500, new { mensagem = "Erro ao processar o login" });
            }
        }

        private string GerarToken(Model.Solicitante solicitante)
        {
            // Obter a chave secreta das configurações
            var chave = _configuration["JwtConfig:Secret"];

            if (string.IsNullOrEmpty(chave))
            {
                throw new InvalidOperationException("Chave JWT não configurada");
            }

            var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
            var credenciais = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256);

            // Criar as claims (afirmações) para o token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, solicitante.Id.ToString()),
                new Claim(ClaimTypes.Name, solicitante.Nome),
                new Claim("CPF", solicitante.CPF)
            };

            // Configurar o token
            var token = new JwtSecurityToken(
                issuer: null, // Simplificar para testes
                audience: null, // Simplificar para testes
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credenciais
            );

            // Retornar o token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
