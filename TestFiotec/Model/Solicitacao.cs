using TestFiotec.Model;

namespace TestFiotec.Model
{
    public class Solicitacao
    {
        public int Id { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string Arbovirose { get; set; } = string.Empty;
        public int SolicitanteId { get; set; }

        public virtual Solicitante Solicitante { get; set; } = null!;
        public int SemanaInicio { get; set; }
        public int SemanaFim { get; set; }
        public string CodigoIBGE { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;

        public virtual ICollection<DadosEpidemiologicos> DadosEpidemiologicos { get; set; } = new List<DadosEpidemiologicos>();
    }
}
