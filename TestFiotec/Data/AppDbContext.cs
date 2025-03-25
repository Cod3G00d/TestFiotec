// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using TestFiotec.Model;

namespace TestFiotec.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Solicitante> Solicitantes { get; set; }
        public DbSet<Solicitacao> Solicitacoes { get; set; }
        public DbSet<DadosEpidemiologicos> DadosEpidemiologicos { get; set; } = null!;


    }
}
