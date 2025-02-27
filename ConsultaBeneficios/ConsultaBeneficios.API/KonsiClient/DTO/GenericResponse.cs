using System.Text.Json.Serialization;

namespace ConsultaBeneficios.API.KonsiClient.DTO
{
    public class GenericResponse<T> where T : class
    {
        [JsonPropertyName("success")]
        public bool Sucesso { get; set; }
        [JsonPropertyName("data")]
        public T? Dados { get; set; }
        [JsonPropertyName("observations")]
        public string? Observacoes { get; set; }
    }
}
