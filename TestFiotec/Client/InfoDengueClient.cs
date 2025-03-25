using System;
using System.Net.Http.Json;
using TestFiotec.DTOs;
using TestFiotec.Model;

namespace TestFiotec.Clients
{
    public class InfoDengueClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InfoDengueClient> _logger;

        public InfoDengueClient(HttpClient httpClient, ILogger<InfoDengueClient> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://info.dengue.mat.br/api/");
            _logger = logger;
        }

        public async Task<List<DadosEpidemiologicos>> ConsultarDadosEpidemiologicos(InfoDengueParametrosDTO parametros)
        {
            try
            {
                // Constrói a URL com os parâmetros fornecidos
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(parametros.Geocode))
                    queryParams.Add($"geocode={parametros.Geocode}");

                if (!string.IsNullOrEmpty(parametros.Disease))
                    queryParams.Add($"disease={parametros.Disease}");

                queryParams.Add($"format={parametros.Format}");

                if (parametros.EwStart.HasValue)
                    queryParams.Add($"ew_start={parametros.EwStart}");

                if (parametros.EwEnd.HasValue)
                    queryParams.Add($"ew_end={parametros.EwEnd}");

                if (parametros.EyStart.HasValue)
                    queryParams.Add($"ey_start={parametros.EyStart}");

                if (parametros.EyEnd.HasValue)
                    queryParams.Add($"ey_end={parametros.EyEnd}");

                var queryString = string.Join("&", queryParams);
                var url = $"https://info.dengue.mat.br/api/alertcity?{queryString}";

                _logger.LogInformation($"Consultando API: {url}");

                var response = await _httpClient.GetAsync(url.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var dados = await response.Content.ReadFromJsonAsync<List<DadosEpidemiologicos>>();
                    return dados ?? new List<DadosEpidemiologicos>();
                }

                _logger.LogError($"Erro ao consultar a API INFODENGUE: {response.StatusCode}");
                return new List<DadosEpidemiologicos>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar a API INFODENGUE");
                return new List<DadosEpidemiologicos>();
            }
        }
    }
}
