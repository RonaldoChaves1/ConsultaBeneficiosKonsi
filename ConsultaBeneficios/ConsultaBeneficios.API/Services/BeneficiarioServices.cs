using ConsultaBeneficios.API.Interfaces;
using ConsultaBeneficios.API.KonsiClient;
using ConsultaBeneficios.API.KonsiClient.DTO;
using ConsultaBeneficios.API.Models;

namespace ConsultaBeneficios.API.Services
{
    public class BeneficiarioServices : IBeneficiarioServices
    {
        private readonly KonsiApiClient _konsiApiClient;
        private readonly IElasticServices _elasticServices;
        private readonly ILogger<BeneficiarioServices> _logger;

        public BeneficiarioServices(KonsiApiClient konsiApiClient, IElasticServices elasticServices, ILogger<BeneficiarioServices> logger)
        {
            _konsiApiClient = konsiApiClient;
            _elasticServices = elasticServices;
            _logger = logger;
        }

        public async Task<Beneficiario> ConsultarBeneficiarioAsync(string cpf)
        {
            try
            {
                var beneficiarioResponse = await _konsiApiClient.ConsultarBeneficio(cpf);

                Beneficiario beneficiario = ToModel(beneficiarioResponse);

                await _elasticServices.AddOrUpdate(beneficiario);

                return beneficiario;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("CPF: {0} - {1}", cpf, ex.Message));
                throw;
            }
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

        public async Task<Beneficiario> BuscarBeneficiarioAsync(string cpf)
        {
            return await _elasticServices.Get(cpf);
        }

        public async Task<List<Beneficiario>?> ListarTodosBeneficiariosAsync()
        {
            return await _elasticServices.GetAll();
        }
    }
}
