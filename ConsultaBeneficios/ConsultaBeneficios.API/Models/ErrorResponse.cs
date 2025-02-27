namespace ConsultaBeneficios.API.Models
{
    public class ErrorResponse
    {
        public bool Sucesso { get => false; }
        public string? Observacoes { get; set; }
    }
}
