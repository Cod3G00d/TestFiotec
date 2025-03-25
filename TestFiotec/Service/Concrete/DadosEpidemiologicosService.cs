using TestFiotec.Model;
using TestFiotec.Repositories.Interface;
using TestFiotec.Services.Interface;

namespace TestFiotec.Services.Concrete
{
    public class DadosEpidemiologicosService : IDadosEpidemiologicosService
    {
        private readonly IRepository<DadosEpidemiologicos> _repository;

        public DadosEpidemiologicosService(IRepository<DadosEpidemiologicos> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DadosEpidemiologicos>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<DadosEpidemiologicos> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(DadosEpidemiologicos solicitante)
        {
            await _repository.AddAsync(solicitante);
        }

        public async Task UpdateAsync(DadosEpidemiologicos solicitante)
        {
            await _repository.UpdateAsync(solicitante);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
