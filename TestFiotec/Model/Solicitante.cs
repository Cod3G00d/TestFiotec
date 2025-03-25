// Models/Solicitante.cs
using TestFiotec.Model;

namespace TestFiotec.Model
{
    public class Solicitante
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
    }
}
