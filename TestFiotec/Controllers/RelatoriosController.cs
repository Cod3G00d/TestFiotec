using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using System.Collections.Generic;using System.Security.Claims;using TestFiotec.Clients;using TestFiotec.DTOs;using TestFiotec.Model;using TestFiotec.Services.Interface;namespace TestFiotec.Controllers{    public enum DiseaseTipo    {        dengue,        zika,        chikungunya    }    [ApiController]    [Route("api/[controller]")]    public class RelatoriosController : ControllerBase    {        private readonly InfoDengueClient _infoDengueClient;        private readonly IDadosEpidemiologicosService _dadosEpidemiologicosService;        private readonly ISolicitacaoService _solicitacaoService;        private readonly ISolicitanteService _solicitanteService;        private readonly ILogger<RelatoriosController> _logger;        public RelatoriosController(            InfoDengueClient infoDengueClient,            IDadosEpidemiologicosService dadosEpidemiologicosService,            ISolicitacaoService solicitacaoService,            ISolicitanteService solicitanteService,            ILogger<RelatoriosController> logger)        {            _infoDengueClient = infoDengueClient;            _dadosEpidemiologicosService = dadosEpidemiologicosService;            _solicitacaoService = solicitacaoService;            _solicitanteService = solicitanteService;            _logger = logger;        }























        /// <summary>        /// Gera um relatório comparativo entre dados epidemiológicos de múltiplos municípios        /// </summary>        /// <param name="geocodes">Lista de códigos IBGE separados por vírgula (ex: "3550308,3304557")</param>        /// <param name="disease">Tipo de doença (Dengue, Chikungunya, Zika)</param>        /// <param name="format">Formato de saída (json, csv)</param>        /// <param name="ewStart">Semana epidemiológica inicial (1-53)</param>        /// <param name="ewEnd">Semana epidemiológica final (1-53)</param>        /// <param name="eyStart">Ano inicial</param>        /// <param name="eyEnd">Ano final</param>        /// <returns>Relatório comparativo dos dados epidemiológicos dos municípios</returns>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             [HttpGet("GerarRelatorioMunicipios")]        public async Task<IActionResult> GerarRelatorioMunicipios(
          [FromQuery] string geocodes,
          [FromQuery] DiseaseTipo disease = DiseaseTipo.dengue,
          [FromQuery] string format = "json",
          [FromQuery] int? ewStart = null,
          [FromQuery] int? ewEnd = null,
          [FromQuery] int? eyStart = null,
          [FromQuery] int? eyEnd = null)        {            try            {
                // Validação do geocodes
                if (string.IsNullOrWhiteSpace(geocodes))                {                    return BadRequest(new { mensagem = "É necessário informar pelo menos um código IBGE (geocode)" });                }                _logger.LogInformation($"Gerando relatório para municípios: {geocodes}, doença: {disease}");

                // Divide a string de geocodes em uma lista
                var geocodeList = geocodes.Split(',').Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g)).ToList();                if (!geocodeList.Any())                {                    return BadRequest(new { mensagem = "Nenhum código IBGE válido foi informado" });                }

                // Lista para armazenar todos os resultados
                var todosResultados = new List<DadosEpidemiologicos>();                var municipiosNaoEncontrados = new List<string>();

                // Realiza uma chamada para cada geocode
                foreach (var geocode in geocodeList)                {                    var parametros = new InfoDengueParametrosDTO                    {                        Geocode = geocode,                        Disease = disease.ToString().ToLower(), // Converte enum para string minúscula
                        Format = format,                        EwStart = ewStart,                        EwEnd = ewEnd,                        EyStart = eyStart,                        EyEnd = eyEnd                    };

                    // Chama a API para cada município
                    var dadosMunicipio = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);                    if (dadosMunicipio != null && dadosMunicipio.Any())                    {                        todosResultados.AddRange(dadosMunicipio);                    }                    else                    {                        municipiosNaoEncontrados.Add(geocode);                        _logger.LogWarning($"Nenhum dado encontrado para o município com geocode: {geocode}");                    }                }

                // Verificar se algum dado foi encontrado
                if (!todosResultados.Any())                {                    return NotFound(new                    {                        mensagem = "Nenhum dado encontrado para os municípios informados",                        municipiosNaoEncontrados                    });                }

                // Organiza os dados para incluir informações sobre municípios não encontrados
                var resultado = new                {                    dados = todosResultados,                    totalMunicipios = geocodeList.Count,                    municipiosEncontrados = geocodeList.Count - municipiosNaoEncontrados.Count,                    municipiosNaoEncontrados                };

                // Registrar a solicitação para cada município encontrado
                await RegistrarSolicitacoes(geocodeList, disease.ToString(), ewStart, ewEnd);                return Ok(resultado);            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao gerar relatório de municípios");                return StatusCode(500, new { mensagem = "Erro ao gerar relatório. Detalhes no log do servidor." });            }        }























        /// <summary>        /// Consulta dados epidemiológicos de acordo com os parâmetros informados        /// </summary>        /// <param name="geocode">Código IBGE da cidade</param>        /// <param name="disease">Tipo de doença (Dengue, Chikungunya, Zika)</param>        /// <param name="format">Formato de saída (json, csv)</param>        /// <param name="ewStart">Semana epidemiológica inicial (1-53)</param>        /// <param name="ewEnd">Semana epidemiológica final (1-53)</param>        /// <param name="eyStart">Ano inicial</param>        /// <param name="eyEnd">Ano final</param>        /// <returns>Lista de dados epidemiológicos</returns>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  [HttpPost("ConsultarDadosEpidemiologicos")]        public async Task<IActionResult> ConsultarDadosEpidemiologicos(
          [FromQuery] string? geocode = null,
          [FromQuery] DiseaseTipo disease = DiseaseTipo.dengue,
          [FromQuery] string format = "json",
          [FromQuery] int? ewStart = null,
          [FromQuery] int? ewEnd = null,
          [FromQuery] int? eyStart = null,
          [FromQuery] int? eyEnd = null)        {            try            {                _logger.LogInformation($"Consultando dados epidemiológicos para doença: {disease}");

                // Criando o DTO com os parâmetros para a chamada da API
                var parametros = new InfoDengueParametrosDTO                {                    Geocode = geocode,                    Disease = disease.ToString().ToLower(), // Converte enum para string minúscula
                    Format = format,                    EwStart = ewStart,                    EwEnd = ewEnd,                    EyStart = eyStart,                    EyEnd = eyEnd                };

                // Chamando a API através do cliente
                var dados = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);                if (dados == null || !dados.Any())                {                    return NotFound(new { mensagem = "Nenhum dado encontrado para os parâmetros informados" });                }

                // Registrar a solicitação
                if (!string.IsNullOrEmpty(geocode))                {                    await RegistrarSolicitacao(geocode, disease.ToString(), ewStart, ewEnd);                }                return Ok(dados);            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao consultar dados epidemiológicos");                return StatusCode(500, new { mensagem = "Erro ao consultar dados epidemiológicos. Detalhes no log do servidor." });            }        }





















        /// <summary>        /// Gera um relatório epidemiológico para as três arboviroses (dengue, zika e chikungunya) em um único município        /// </summary>        /// <param name="geocode">Código IBGE da cidade</param>        /// <param name="format">Formato de saída (json, csv)</param>        /// <param name="ewStart">Semana epidemiológica inicial (1-53)</param>        /// <param name="ewEnd">Semana epidemiológica final (1-53)</param>        /// <param name="eyStart">Ano inicial</param>        /// <param name="eyEnd">Ano final</param>        /// <returns>Relatório comparativo das três arboviroses para o município</returns>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 [HttpGet("GerarRelatorioPorArbovirose")]        public async Task<IActionResult> GerarRelatorioPorArbovirose(
          [FromQuery] string geocode,
          [FromQuery] string format = "json",
          [FromQuery] int? ewStart = null,
          [FromQuery] int? ewEnd = null,
          [FromQuery] int? eyStart = null,
          [FromQuery] int? eyEnd = null)        {            try            {
                // Validação do geocode
                if (string.IsNullOrWhiteSpace(geocode))                {                    return BadRequest(new { mensagem = "É necessário informar o código IBGE (geocode) do município" });                }                _logger.LogInformation($"Gerando relatório por arbovirose para o município: {geocode}");

                // Dicionário para armazenar os resultados de cada doença
                var resultadosPorDoenca = new Dictionary<string, List<DadosEpidemiologicos>>();

                // Realiza uma chamada para cada tipo de doença
                foreach (DiseaseTipo disease in Enum.GetValues(typeof(DiseaseTipo)))                {                    var parametros = new InfoDengueParametrosDTO                    {                        Geocode = geocode,                        Disease = disease.ToString(),                        Format = format,                        EwStart = ewStart,                        EwEnd = ewEnd,                        EyStart = eyStart,                        EyEnd = eyEnd                    };

                    // Chama a API para a combinação de município e doença
                    var dadosDoenca = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);                    if (dadosDoenca != null && dadosDoenca.Any())                    {                        resultadosPorDoenca.Add(disease.ToString(), dadosDoenca);                        _logger.LogInformation($"Encontrados {dadosDoenca.Count} registros para {disease} no município {geocode}");

                        // Registrar solicitação para cada doença
                        await RegistrarSolicitacao(geocode, disease.ToString(), ewStart, ewEnd);                    }                    else                    {                        resultadosPorDoenca.Add(disease.ToString(), new List<DadosEpidemiologicos>());                        _logger.LogWarning($"Nenhum dado encontrado para {disease} no município com geocode: {geocode}");                    }                }

                // Verificar se algum dado foi encontrado para pelo menos uma doença
                bool algumDadoEncontrado = resultadosPorDoenca.Values.Any(list => list.Any());                if (!algumDadoEncontrado)                {                    return NotFound(new                    {                        mensagem = "Nenhum dado epidemiológico encontrado para o município informado",                        geocode = geocode                    });                }

                // Organiza os dados para o relatório
                var resultado = new                {                    geocode = geocode,                    dengue = resultadosPorDoenca.ContainsKey(DiseaseTipo.dengue.ToString())
? resultadosPorDoenca[DiseaseTipo.dengue.ToString()]
: new List<DadosEpidemiologicos>(),                    zika = resultadosPorDoenca.ContainsKey(DiseaseTipo.zika.ToString())
? resultadosPorDoenca[DiseaseTipo.zika.ToString()]
: new List<DadosEpidemiologicos>(),                    chikungunya = resultadosPorDoenca.ContainsKey(DiseaseTipo.chikungunya.ToString())
? resultadosPorDoenca[DiseaseTipo.chikungunya.ToString()]
: new List<DadosEpidemiologicos>(),                    resumo = new                    {                        totalRegistrosDengue = resultadosPorDoenca.ContainsKey(DiseaseTipo.dengue.ToString())
? resultadosPorDoenca[DiseaseTipo.dengue.ToString()].Count
: 0,                        totalRegistrosZika = resultadosPorDoenca.ContainsKey(DiseaseTipo.zika.ToString())
? resultadosPorDoenca[DiseaseTipo.zika.ToString()].Count
: 0,                        totalRegistrosChikungunya = resultadosPorDoenca.ContainsKey(DiseaseTipo.chikungunya.ToString())
? resultadosPorDoenca[DiseaseTipo.chikungunya.ToString()].Count
: 0                    }                };                return Ok(resultado);            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao gerar relatório por arbovirose");                return StatusCode(500, new { mensagem = "Erro ao gerar relatório. Detalhes no log do servidor." });            }        }







        /// <summary>        /// Método interno para registrar uma solicitação        /// </summary>                                                                                                               private async Task RegistrarSolicitacao(string codigoIBGE, string arbovirose, int? semanaInicio, int? semanaFim)        {            try            {
                // Obter o ID do solicitante a partir do token de autenticação
                var solicitanteId = ObterSolicitanteIdDoToken();                if (!solicitanteId.HasValue)                {                    _logger.LogWarning("Não foi possível obter o ID do solicitante para registrar a solicitação");                    return;                }

                // Criar objeto de solicitação
                var solicitacao = new Solicitacao                {                    DataSolicitacao = DateTime.Now,                    Arbovirose = arbovirose,                    SolicitanteId = solicitanteId.Value,                    SemanaInicio = semanaInicio ?? 1,                    SemanaFim = semanaFim ?? 53,                    CodigoIBGE = codigoIBGE                };

                // Salvar a solicitação
                await _solicitacaoService.CreateAsync(solicitacao);                _logger.LogInformation($"Solicitação registrada com sucesso para o município {codigoIBGE}, doença {arbovirose}");            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao registrar solicitação");            }        }







        /// <summary>        /// Método para registrar múltiplas solicitações de uma vez        /// </summary>                                                                                                                         private async Task RegistrarSolicitacoes(List<string> codigosIBGE, string arbovirose, int? semanaInicio, int? semanaFim)        {            foreach (var codigoIBGE in codigosIBGE)            {                await RegistrarSolicitacao(codigoIBGE, arbovirose, semanaInicio, semanaFim);            }        }







        /// <summary>        /// Obtém o ID do solicitante a partir do token JWT        /// </summary>                                                                                                                 private int? ObterSolicitanteIdDoToken()        {            var identity = HttpContext.User.Identity as ClaimsIdentity;            if (identity == null)            {                return null;            }            var idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);            if (idClaim == null || !int.TryParse(idClaim.Value, out int solicitanteId))            {                return null;            }            return solicitanteId;        }













        /// <summary>        /// Método privado para salvar dados epidemiológicos no banco de dados        /// </summary>                                                                                                                                                                                                                                                                private async Task SalvarDadosEpidemiologicos(List<DadosEpidemiologicos> dados, int? solicitacaoId = null)
        {
            try
            {
                if (dados == null || !dados.Any())
                {
                    return;
                }

                foreach (var dado in dados)
                {
                    // Se tiver um ID de solicitação, associe aos dados
                    if (solicitacaoId.HasValue)
                    {
                        dado.SolicitacaoId = solicitacaoId.Value;
                    }

                    // Salva no banco de dados
                    await _dadosEpidemiologicosService.AddAsync(dado);
                }

                _logger.LogInformation($"Salvos {dados.Count} registros de dados epidemiológicos no banco de dados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar dados epidemiológicos no banco de dados");
            }
        }

        /// <summary>
        /// Obtém o nome do município a partir do código IBGE
        /// </summary>
        private string ObteMunicipioPorIBGE(string codigoIBGE)
        {
            // Implementação simples - em um sistema real, isso consultaria uma base de dados de municípios
            var municipiosPrincipais = new Dictionary<string, string>
    {
        {"3550308", "São Paulo"},
        {"3304557", "Rio de Janeiro"},
        {"2304400", "Fortaleza"},
        {"3106200", "Belo Horizonte"},
        {"5300108", "Brasília"},
        {"2927408", "Salvador"},
        {"1302603", "Manaus"},
        {"2611606", "Recife"},
        {"4106902", "Curitiba"},
        {"4314902", "Porto Alegre"}
    };

            if (municipiosPrincipais.TryGetValue(codigoIBGE, out string nome))
            {
                return nome;
            }

            return $"Município {codigoIBGE}"; // Nome genérico quando não encontrar
        }    }}