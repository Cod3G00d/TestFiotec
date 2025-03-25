// DTOs/InfoDengueParametrosDTO.cs
namespace TestFiotec.DTOs
{
    public class InfoDengueParametrosDTO
    {
        /// <summary>
        /// C�digo IBGE da cidade
        /// </summary>
        public string? Geocode { get; set; }
        
        /// <summary>
        /// Tipo de doen�a a ser consultado (dengue|chikungunya|zika)
        /// </summary>
        public string? Disease { get; set; }
        
        /// <summary>
        /// Formato de sa�da dos dados (json|csv)
        /// </summary>
        public string Format { get; set; } = "json";
        
        /// <summary>
        /// Semana epidemiol�gica de in�cio da consulta (1-53)
        /// </summary>
        public int? EwStart { get; set; }
        
        /// <summary>
        /// Semana epidemiol�gica de t�rmino da consulta (1-53)
        /// </summary>
        public int? EwEnd { get; set; }
        
        /// <summary>
        /// Ano de in�cio da consulta
        /// </summary>
        public int? EyStart { get; set; }
        
        /// <summary>
        /// Ano de t�rmino da consulta
        /// </summary>
        public int? EyEnd { get; set; }


    }
}
