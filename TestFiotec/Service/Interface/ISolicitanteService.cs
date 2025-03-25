using TestFiotec.Model;

namespace TestFiotec.Services.Interface
{
    public interface ISolicitanteService
    {
        Task<IEnumerable<Solicitante>> GetAllAsync();
        Task<Solicitante> GetByIdAsync(int id);
        Task<Solicitante> GetByCPFAsync(string cpf);
        Task AddAsync(Solicitante solicitante);
        Task UpdateAsync(Solicitante solicitante);
        Task DeleteAsync(int id);
    }
}
