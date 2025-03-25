using TestFiotec.Model;

namespace TestFiotec.Services.Interface
{
    public interface IDadosEpidemiologicosService
    {
        Task<IEnumerable<DadosEpidemiologicos>> GetAllAsync();
        Task<DadosEpidemiologicos> GetByIdAsync(int id);
        Task AddAsync(DadosEpidemiologicos solicitante);
        Task UpdateAsync(DadosEpidemiologicos solicitante);
        Task DeleteAsync(int id);
    }
}
