using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestFiotec.Model;
using TestFiotec.Repositories.Interface;
using TestFiotec.Services.Interface;

namespace TestFiotec.Services.Concrete
{
    public class SolicitanteService : ISolicitanteService
    {
        private readonly IRepository<Solicitante> _repository;
        private readonly ILogger<SolicitanteService> _logger;

        public SolicitanteService(IRepository<Solicitante> repository, ILogger<SolicitanteService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Solicitante>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Solicitante> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Solicitante> GetByCPFAsync(string cpf)
        {
            var solicitantes = await _repository.FindAsync(s => s.CPF == cpf);
            return solicitantes.FirstOrDefault();
        }

        public async Task AddAsync(Solicitante solicitante)
        {
            await _repository.AddAsync(solicitante);
        }

        public async Task UpdateAsync(Solicitante solicitante)
        {
            await _repository.UpdateAsync(solicitante);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
