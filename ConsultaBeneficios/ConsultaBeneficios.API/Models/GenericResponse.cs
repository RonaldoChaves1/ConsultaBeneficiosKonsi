using System.Text.Json.Serialization;

namespace ConsultaBeneficios.API.Models
{
    public class GenericResponse<T> where T : class
    {
        public bool Sucesso { get => true; }
        public T? Dados { get; set; }
    }
}
