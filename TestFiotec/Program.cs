using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TestFiotec.Clients;
using TestFiotec.Data;
using TestFiotec.Repositories.Concrete;
using TestFiotec.Repositories.Interface;
using TestFiotec.Services.Concrete;
using TestFiotec.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do JWT no Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API INFODENGUE",
        Version = "v1",
        Description = "API para consulta de dados epidemiológicos da INFODENGUE",
    });

    // Adicionar configuração para autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração do DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do JWT
var jwtKey = builder.Configuration["JwtConfig:Secret"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Chave JWT não configurada corretamente no appsettings.json");
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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = false, // Simplificar para testes - em produção deve ser true
        ValidateAudience = false, // Simplificar para testes - em produção deve ser true
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Reduzir tolerância de tempo
    };
});

// Registro de serviços
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ISolicitanteService, SolicitanteService>();
builder.Services.AddScoped<IDadosEpidemiologicosService, DadosEpidemiologicosService>();
builder.Services.AddScoped<ISolicitacaoService, SolicitacaoService>();

// Configuração do HttpClient para a API INFODENGUE
builder.Services.AddHttpClient<InfoDengueClient>();

var app = builder.Build();

// Configure o pipeline de HTTP request
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API INFODENGUE v1");
    c.RoutePrefix = string.Empty; // Para servir a UI do Swagger na raiz
});

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
