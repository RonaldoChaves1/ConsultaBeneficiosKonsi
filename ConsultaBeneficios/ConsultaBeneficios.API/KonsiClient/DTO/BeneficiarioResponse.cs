using ConsultaBeneficios.API.Models;
using System.Text.Json.Serialization;

namespace ConsultaBeneficios.API.KonsiClient.DTO
{
    public class BeneficiarioResponse
    {
        [JsonPropertyName("cpf")]
        public string? CPF { get; set; }
        [JsonPropertyName("beneficios")]
        public List<BeneficioResponse>? Beneficios { get; set; }
    }
}
