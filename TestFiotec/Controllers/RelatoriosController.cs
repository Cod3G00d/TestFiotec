using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using System.Collections.Generic;using System.Security.Claims;using TestFiotec.Clients;using TestFiotec.DTOs;using TestFiotec.Model;using TestFiotec.Services.Interface;namespace TestFiotec.Controllers{    public enum DiseaseTipo    {        dengue,        zika,        chikungunya    }    [ApiController]    [Route("api/[controller]")]    public class RelatoriosController : ControllerBase    {        private readonly InfoDengueClient _infoDengueClient;        private readonly IDadosEpidemiologicosService _dadosEpidemiologicosService;        private readonly ISolicitacaoService _solicitacaoService;        private readonly ISolicitanteService _solicitanteService;        private readonly ILogger<RelatoriosController> _logger;        public RelatoriosController(            InfoDengueClient infoDengueClient,            IDadosEpidemiologicosService dadosEpidemiologicosService,            ISolicitacaoService solicitacaoService,            ISolicitanteService solicitanteService,            ILogger<RelatoriosController> logger)        {            _infoDengueClient = infoDengueClient;            _dadosEpidemiologicosService = dadosEpidemiologicosService;            _solicitacaoService = solicitacaoService;            _solicitanteService = solicitanteService;            _logger = logger;        }























        /// <summary>        /// Gera um relat�rio comparativo entre dados epidemiol�gicos de m�ltiplos munic�pios        /// </summary>        /// <param name="geocodes">Lista de c�digos IBGE separados por v�rgula (ex: "3550308,3304557")</param>        /// <param name="disease">Tipo de doen�a (Dengue, Chikungunya, Zika)</param>        /// <param name="format">Formato de sa�da (json, csv)</param>        /// <param name="ewStart">Semana epidemiol�gica inicial (1-53)</param>        /// <param name="ewEnd">Semana epidemiol�gica final (1-53)</param>        /// <param name="eyStart">Ano inicial</param>        /// <param name="eyEnd">Ano final</param>        /// <returns>Relat�rio comparativo dos dados epidemiol�gicos dos munic�pios</returns>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             [HttpGet("GerarRelatorioMunicipios")]        public async Task<IActionResult> GerarRelatorioMunicipios(
          [FromQuery] string geocodes,
          [FromQuery] DiseaseTipo disease = DiseaseTipo.dengue,
          [FromQuery] string format = "json",
          [FromQuery] int? ewStart = null,
          [FromQuery] int? ewEnd = null,
          [FromQuery] int? eyStart = null,
          [FromQuery] int? eyEnd = null)        {            try            {
                // Valida��o do geocodes
                if (string.IsNullOrWhiteSpace(geocodes))                {                    return BadRequest(new { mensagem = "� necess�rio informar pelo menos um c�digo IBGE (geocode)" });                }                _logger.LogInformation($"Gerando relat�rio para munic�pios: {geocodes}, doen�a: {disease}");

                // Divide a string de geocodes em uma lista
                var geocodeList = geocodes.Split(',').Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g)).ToList();                if (!geocodeList.Any())                {                    return BadRequest(new { mensagem = "Nenhum c�digo IBGE v�lido foi informado" });                }

                // Lista para armazenar todos os resultados
                var todosResultados = new List<DadosEpidemiologicos>();                var municipiosNaoEncontrados = new List<string>();

                // Realiza uma chamada para cada geocode
                foreach (var geocode in geocodeList)                {                    var parametros = new InfoDengueParametrosDTO                    {                        Geocode = geocode,                        Disease = disease.ToString().ToLower(), // Converte enum para string min�scula
                        Format = format,                        EwStart = ewStart,                        EwEnd = ewEnd,                        EyStart = eyStart,                        EyEnd = eyEnd                    };

                    // Chama a API para cada munic�pio
                    var dadosMunicipio = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);                    if (dadosMunicipio != null && dadosMunicipio.Any())                    {                        todosResultados.AddRange(dadosMunicipio);                    }                    else                    {                        municipiosNaoEncontrados.Add(geocode);                        _logger.LogWarning($"Nenhum dado encontrado para o munic�pio com geocode: {geocode}");                    }                }

                // Verificar se algum dado foi encontrado
                if (!todosResultados.Any())                {                    return NotFound(new                    {                        mensagem = "Nenhum dado encontrado para os munic�pios informados",                        municipiosNaoEncontrados                    });                }

                // Organiza os dados para incluir informa��es sobre munic�pios n�o encontrados
                var resultado = new                {                    dados = todosResultados,                    totalMunicipios = geocodeList.Count,                    municipiosEncontrados = geocodeList.Count - municipiosNaoEncontrados.Count,                    municipiosNaoEncontrados                };

                // Registrar a solicita��o para cada munic�pio encontrado
                await RegistrarSolicitacoes(geocodeList, disease.ToString(), ewStart, ewEnd);                return Ok(resultado);            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao gerar relat�rio de munic�pios");                return StatusCode(500, new { mensagem = "Erro ao gerar relat�rio. Detalhes no log do servidor." });            }        }























        /// <summary>        /// Consulta dados epidemiol�gicos de acordo com os par�metros informados        /// </summary>        /// <param name="geocode">C�digo IBGE da cidade</param>        /// <param name="disease">Tipo de doen�a (Dengue, Chikungunya, Zika)</param>        /// <param name="format">Formato de sa�da (json, csv)</param>        /// <param name="ewStart">Semana epidemiol�gica inicial (1-53)</param>        /// <param name="ewEnd">Semana epidemiol�gica final (1-53)</param>        /// <param name="eyStart">Ano inicial</param>        /// <param name="eyEnd">Ano final</param>        /// <returns>Lista de dados epidemiol�gicos</returns>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  [HttpPost("ConsultarDadosEpidemiologicos")]        public async Task<IActionResult> ConsultarDadosEpidemiologicos(
          [FromQuery] string? geocode = null,
          [FromQuery] DiseaseTipo disease = DiseaseTipo.dengue,
          [FromQuery] string format = "json",
          [FromQuery] int? ewStart = null,
          [FromQuery] int? ewEnd = null,
          [FromQuery] int? eyStart = null,
          [FromQuery] int? eyEnd = null)        {            try            {                _logger.LogInformation($"Consultando dados epidemiol�gicos para doen�a: {disease}");

                // Criando o DTO com os par�metros para a chamada da API
                var parametros = new InfoDengueParametrosDTO                {                    Geocode = geocode,                    Disease = disease.ToString().ToLower(), // Converte enum para string min�scula
                    Format = format,                    EwStart = ewStart,                    EwEnd = ewEnd,                    EyStart = eyStart,                    EyEnd = eyEnd                };

                // Chamando a API atrav�s do cliente
                var dados = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);                if (dados == null || !dados.Any())                {                    return NotFound(new { mensagem = "Nenhum dado encontrado para os par�metros informados" });                }

                // Registrar a solicita��o
                if (!string.IsNullOrEmpty(geocode))                {                    await RegistrarSolicitacao(geocode, disease.ToString(), ewStart, ewEnd);                }                return Ok(dados);            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao consultar dados epidemiol�gicos");                return StatusCode(500, new { mensagem = "Erro ao consultar dados epidemiol�gicos. Detalhes no log do servidor." });            }        }





















        /// <summary>        /// Gera um relat�rio epidemiol�gico para as tr�s arboviroses (dengue, zika e chikungunya) em um �nico munic�pio        /// </summary>        /// <param name="geocode">C�digo IBGE da cidade</param>        /// <param name="format">Formato de sa�da (json, csv)</param>        /// <param name="ewStart">Semana epidemiol�gica inicial (1-53)</param>        /// <param name="ewEnd">Semana epidemiol�gica final (1-53)</param>        /// <param name="eyStart">Ano inicial</param>        /// <param name="eyEnd">Ano final</param>        /// <returns>Relat�rio comparativo das tr�s arboviroses para o munic�pio</returns>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 [HttpGet("GerarRelatorioPorArbovirose")]        public async Task<IActionResult> GerarRelatorioPorArbovirose(
          [FromQuery] string geocode,
          [FromQuery] string format = "json",
          [FromQuery] int? ewStart = null,
          [FromQuery] int? ewEnd = null,
          [FromQuery] int? eyStart = null,
          [FromQuery] int? eyEnd = null)        {            try            {
                // Valida��o do geocode
                if (string.IsNullOrWhiteSpace(geocode))                {                    return BadRequest(new { mensagem = "� necess�rio informar o c�digo IBGE (geocode) do munic�pio" });                }                _logger.LogInformation($"Gerando relat�rio por arbovirose para o munic�pio: {geocode}");

                // Dicion�rio para armazenar os resultados de cada doen�a
                var resultadosPorDoenca = new Dictionary<string, List<DadosEpidemiologicos>>();

                // Realiza uma chamada para cada tipo de doen�a
                foreach (DiseaseTipo disease in Enum.GetValues(typeof(DiseaseTipo)))                {                    var parametros = new InfoDengueParametrosDTO                    {                        Geocode = geocode,                        Disease = disease.ToString(),                        Format = format,                        EwStart = ewStart,                        EwEnd = ewEnd,                        EyStart = eyStart,                        EyEnd = eyEnd                    };

                    // Chama a API para a combina��o de munic�pio e doen�a
                    var dadosDoenca = await _infoDengueClient.ConsultarDadosEpidemiologicos(parametros);                    if (dadosDoenca != null && dadosDoenca.Any())                    {                        resultadosPorDoenca.Add(disease.ToString(), dadosDoenca);                        _logger.LogInformation($"Encontrados {dadosDoenca.Count} registros para {disease} no munic�pio {geocode}");

                        // Registrar solicita��o para cada doen�a
                        await RegistrarSolicitacao(geocode, disease.ToString(), ewStart, ewEnd);                    }                    else                    {                        resultadosPorDoenca.Add(disease.ToString(), new List<DadosEpidemiologicos>());                        _logger.LogWarning($"Nenhum dado encontrado para {disease} no munic�pio com geocode: {geocode}");                    }                }

                // Verificar se algum dado foi encontrado para pelo menos uma doen�a
                bool algumDadoEncontrado = resultadosPorDoenca.Values.Any(list => list.Any());                if (!algumDadoEncontrado)                {                    return NotFound(new                    {                        mensagem = "Nenhum dado epidemiol�gico encontrado para o munic�pio informado",                        geocode = geocode                    });                }

                // Organiza os dados para o relat�rio
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
: 0                    }                };                return Ok(resultado);            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao gerar relat�rio por arbovirose");                return StatusCode(500, new { mensagem = "Erro ao gerar relat�rio. Detalhes no log do servidor." });            }        }







        /// <summary>        /// M�todo interno para registrar uma solicita��o        /// </summary>                                                                                                               private async Task RegistrarSolicitacao(string codigoIBGE, string arbovirose, int? semanaInicio, int? semanaFim)        {            try            {
                // Obter o ID do solicitante a partir do token de autentica��o
                var solicitanteId = ObterSolicitanteIdDoToken();                if (!solicitanteId.HasValue)                {                    _logger.LogWarning("N�o foi poss�vel obter o ID do solicitante para registrar a solicita��o");                    return;                }

                // Criar objeto de solicita��o
                var solicitacao = new Solicitacao                {                    DataSolicitacao = DateTime.Now,                    Arbovirose = arbovirose,                    SolicitanteId = solicitanteId.Value,                    SemanaInicio = semanaInicio ?? 1,                    SemanaFim = semanaFim ?? 53,                    CodigoIBGE = codigoIBGE                };

                // Salvar a solicita��o
                await _solicitacaoService.CreateAsync(solicitacao);                _logger.LogInformation($"Solicita��o registrada com sucesso para o munic�pio {codigoIBGE}, doen�a {arbovirose}");            }            catch (Exception ex)            {                _logger.LogError(ex, "Erro ao registrar solicita��o");            }        }







        /// <summary>        /// M�todo para registrar m�ltiplas solicita��es de uma vez        /// </summary>                                                                                                                         private async Task RegistrarSolicitacoes(List<string> codigosIBGE, string arbovirose, int? semanaInicio, int? semanaFim)        {            foreach (var codigoIBGE in codigosIBGE)            {                await RegistrarSolicitacao(codigoIBGE, arbovirose, semanaInicio, semanaFim);            }        }







        /// <summary>        /// Obt�m o ID do solicitante a partir do token JWT        /// </summary>                                                                                                                 private int? ObterSolicitanteIdDoToken()        {            var identity = HttpContext.User.Identity as ClaimsIdentity;            if (identity == null)            {                return null;            }            var idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);            if (idClaim == null || !int.TryParse(idClaim.Value, out int solicitanteId))            {                return null;            }            return solicitanteId;        }













        /// <summary>        /// M�todo privado para salvar dados epidemiol�gicos no banco de dados        /// </summary>                                                                                                                                                                                                                                                                private async Task SalvarDadosEpidemiologicos(List<DadosEpidemiologicos> dados, int? solicitacaoId = null)
        {
            try
            {
                if (dados == null || !dados.Any())
                {
                    return;
                }

                foreach (var dado in dados)
                {
                    // Se tiver um ID de solicita��o, associe aos dados
                    if (solicitacaoId.HasValue)
                    {
                        dado.SolicitacaoId = solicitacaoId.Value;
                    }

                    // Salva no banco de dados
                    await _dadosEpidemiologicosService.AddAsync(dado);
                }

                _logger.LogInformation($"Salvos {dados.Count} registros de dados epidemiol�gicos no banco de dados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar dados epidemiol�gicos no banco de dados");
            }
        }

        /// <summary>
        /// Obt�m o nome do munic�pio a partir do c�digo IBGE
        /// </summary>
        private string ObteMunicipioPorIBGE(string codigoIBGE)
        {
            // Implementa��o simples - em um sistema real, isso consultaria uma base de dados de munic�pios
            var municipiosPrincipais = new Dictionary<string, string>
    {
        {"3550308", "S�o Paulo"},
        {"3304557", "Rio de Janeiro"},
        {"2304400", "Fortaleza"},
        {"3106200", "Belo Horizonte"},
        {"5300108", "Bras�lia"},
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

            return $"Munic�pio {codigoIBGE}"; // Nome gen�rico quando n�o encontrar
        }    }}