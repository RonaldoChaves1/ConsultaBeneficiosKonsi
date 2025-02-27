using System.Text.Json.Serialization;

namespace ConsultaBeneficios.API.KonsiClient.DTO
{
    public class BeneficioResponse
    {
        [JsonPropertyName("numero_beneficio")]
        public string? NumeroBeneficio { get; set; }
        [JsonPropertyName("codigo_tipo_beneficio")]
        public string? CodigoTipoBeneficio { get; set; }
    }
}
