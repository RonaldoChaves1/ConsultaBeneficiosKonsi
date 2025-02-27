using ConsultaBeneficios.API.Interfaces;
using ConsultaBeneficios.API.KonsiClient;
using ConsultaBeneficios.API.KonsiClient.DTO;
using ConsultaBeneficios.API.Models;
using System.Linq;

namespace ConsultaBeneficios.API.Services
{
    public class BeneficiarioServices : IBeneficiarioServices
    {
        private readonly KonsiApiClient _konsiApiClient;

        public BeneficiarioServices(KonsiApiClient konsiApiClient)
        {
            _konsiApiClient = konsiApiClient;
        }

        public async Task<Beneficiario> ConsultarBeneficiarioAsync(string cpf)
        {
            var beneficiarioResponse = await _konsiApiClient.ConsultarBeneficio(cpf);
            Beneficiario beneficiario = ToModel(beneficiarioResponse);

            return beneficiario;
        }

        private static Beneficiario ToModel(BeneficiarioResponse beneficiarioResponse)
        {
            var beneficiario = new Beneficiario();
            beneficiario.CPF = beneficiarioResponse.CPF;

            var beneficios = beneficiarioResponse.Beneficios.Select(beneficioResponse => new Beneficio
            {
                NumeroBeneficio = beneficioResponse.NumeroBeneficio,
                CodigoTipoBeneficio = beneficioResponse.CodigoTipoBeneficio
            }).ToList();

            beneficiario.Beneficios = beneficios;

            return beneficiario;
        }
    }
}
