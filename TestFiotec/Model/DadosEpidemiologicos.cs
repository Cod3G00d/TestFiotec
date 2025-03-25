using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestFiotec.Model
{
    public class DadosEpidemiologicos
    {
        [Key]
        public long DadosEpidemiologicosId { get; set; }

        [JsonPropertyName("casprov_est")]
        public double? CasProvEst { get; set; }

        [JsonPropertyName("casprov_est_min")]
        public double? CasProvEstMin { get; set; }

        [JsonPropertyName("casprov_est_max")]
        public double? CasProvEstMax { get; set; }

        [JsonPropertyName("casconf")]
        public double? CasConf { get; set; }

        [JsonPropertyName("notif_accum_year")]
        public double? NotifAccumYear { get; set; }

        [JsonPropertyName("data_iniSE")]
        public long DataIniSE { get; set; }

        [JsonPropertyName("SE")]
        public double? SE { get; set; }

        [JsonPropertyName("casos_est")]
        public double? CasosEst { get; set; }

        [JsonPropertyName("casos_est_min")]
        public double? CasosEstMin { get; set; }

        [JsonPropertyName("casos_est_max")]
        public double? CasosEstMax { get; set; }

        [JsonPropertyName("casos")]
        public double? Casos { get; set; }

        [JsonPropertyName("p_rt1")]
        public double? PRt1 { get; set; }

        [JsonPropertyName("p_inc100k")]
        public double? PInc100k { get; set; }

        [JsonPropertyName("Localidade_id")]
        public int LocalidadeId { get; set; }

        [JsonPropertyName("nivel")]
        public double? Nivel { get; set; }

        [JsonPropertyName("id")]
        public long IdConsulta { get; set; }

        [JsonPropertyName("versao_modelo")]
        public string VersaoModelo { get; set; } = string.Empty;

        [JsonPropertyName("tweet")]
        public double? Tweet { get; set; }

        [JsonPropertyName("Rt")]
        public double? Rt { get; set; }

        [JsonPropertyName("pop")]
        public double? Pop { get; set; }

        [JsonPropertyName("tempmin")]
        public double? TempMin { get; set; }

        [JsonPropertyName("umidmax")]
        public double? UmidMax { get; set; }

        [JsonPropertyName("receptivo")]
        public double? Receptivo { get; set; }

        [JsonPropertyName("transmissao")]
        public double? Transmissao { get; set; }

        [JsonPropertyName("nivel_inc")]
        public double? NivelInc { get; set; }

        [JsonPropertyName("umidmed")]
        public double? UmidMed { get; set; }

        [JsonPropertyName("umidmin")]
        public double? UmidMin { get; set; }

        [JsonPropertyName("tempmed")]
        public double? TempMed { get; set; }

        [JsonPropertyName("tempmax")]
        public double? TempMax { get; set; }

        [JsonPropertyName("casprov")]
        public double? CasProv { get; set; }

        // Adicionar relação com Solicitacao
        public int? SolicitacaoId { get; set; }
        public virtual Solicitacao? Solicitacao { get; set; }
    }
}
