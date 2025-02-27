using System.Text.Json.Serialization;

namespace ConsultaBeneficios.API.Models
{
    public class Beneficiario
    {
        public string? CPF { get; set; }
        public List<Beneficio>? Beneficios { get; set; }
    }
}
