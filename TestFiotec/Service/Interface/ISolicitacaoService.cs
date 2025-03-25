using TestFiotec.Model;

namespace TestFiotec.Services.Interface
{
    public interface ISolicitacaoService
    {
        Task<Solicitacao> CreateAsync(Solicitacao solicitacao);
        Task<Solicitacao?> GetByIdAsync(int id);
        Task<IEnumerable<Solicitacao>> GetAllAsync();
        Task<List<DadosEpidemiologicos>> ConsultarDadosEpidemiologicosAsync(int solicitacaoId);
    }
}
