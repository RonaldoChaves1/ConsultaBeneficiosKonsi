using ConsultaBeneficios.API.Models;

namespace ConsultaBeneficios.API.Interfaces
{
    public interface IBeneficiarioServices
    {
        Task<Beneficiario> ConsultarBeneficiarioAsync(string cpf);
    }
}
