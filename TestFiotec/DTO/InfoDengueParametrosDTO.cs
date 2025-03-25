// DTOs/InfoDengueParametrosDTO.cs
namespace TestFiotec.DTOs
{
    public class InfoDengueParametrosDTO
    {
        /// <summary>
        /// Código IBGE da cidade
        /// </summary>
        public string? Geocode { get; set; }
        
        /// <summary>
        /// Tipo de doença a ser consultado (dengue|chikungunya|zika)
        /// </summary>
        public string? Disease { get; set; }
        
        /// <summary>
        /// Formato de saída dos dados (json|csv)
        /// </summary>
        public string Format { get; set; } = "json";
        
        /// <summary>
        /// Semana epidemiológica de início da consulta (1-53)
        /// </summary>
        public int? EwStart { get; set; }
        
        /// <summary>
        /// Semana epidemiológica de término da consulta (1-53)
        /// </summary>
        public int? EwEnd { get; set; }
        
        /// <summary>
        /// Ano de início da consulta
        /// </summary>
        public int? EyStart { get; set; }
        
        /// <summary>
        /// Ano de término da consulta
        /// </summary>
        public int? EyEnd { get; set; }


    }
}
