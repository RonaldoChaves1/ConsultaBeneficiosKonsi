using System.Text.Json.Serialization;

namespace ConsultaBeneficios.API.KonsiClient.DTO
{
    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string? Valor { get; set; }
        [JsonPropertyName("type")]
        public string? Tipo { get; set; }
        [JsonPropertyName("expiresIn")]
        public DateTime DataExpiracao { get; set; }
        public bool Valido { get => DataExpiracao > DateTime.Now; }
    }
}
