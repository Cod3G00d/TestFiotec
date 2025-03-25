
// Services/Concrete/SolicitacaoService.cs
using TestFiotec.Clients;
using TestFiotec.DTOs;
using TestFiotec.Model;
using TestFiotec.Repositories.Interface;
using TestFiotec.Services.Interface;

namespace TestFiotec.Services.Concrete
{
    public class SolicitacaoService : ISolicitacaoService
    {
        private readonly IRepository<Solicitacao> _repository;
        private readonly IRepository<DadosEpidemiologicos> _dadosRepository;
        private readonly InfoDengueClient _infoDengueClient;
        private readonly ILogger<SolicitacaoService> _logger;

        public SolicitacaoService(
            IRepository<Solicitacao> repository,
            IRepository<DadosEpidemiologicos> dadosRepository,
            InfoDengueClient infoDengueClient,
            ILogger<SolicitacaoService> logger)
        {
            _repository = repository;
            _dadosRepository = dadosRepository;
            _infoDengueClient = infoDengueClient;
            _logger = logger;
        }

        public async Task<Solicitacao> CreateAsync(Solicitacao solicitacao)
        {
            await _repository.AddAsync(solicitacao);
            return solicitacao;
        }

        public async Task<Solicitacao?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Solicitacao>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<DadosEpidemiologicos>> ConsultarDadosEpidemiologicosAsync(int solicitacaoId)
        {
            // Buscar a solicitação do banco de dados
            var solicitacao = await _repository.GetByIdAsync(solicitacaoId);
            if (solicitacao == null)
            {
                _logger.LogWarning($"Solicitação com ID {solicitacaoId} não encontrada");
                return new List<DadosEpidemiologicos>();
            }

            // Criar o DTO com os parâmetros para a chamada da API
            var parametros = new InfoDengueParametrosDTO
            {
                Geocode = solicitacao.CodigoIBGE,
                Disease = solicitacao.Arbovirose,
                EwStart = solicitacao.SemanaInicio,
                EwEnd = solicitacao.SemanaFim,
                Format = "json"
            };

            // Chamar a API
            var dadosEpidemiologicos = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);

            // Associar os dados à solicitação
            foreach (var dado in dadosEpidemiologicos)
            {
                dado.SolicitacaoId = solicitacaoId;
                await _dadosRepository.AddAsync(dado);
            }

            return dadosEpidemiologicos;
        }
    }
}
