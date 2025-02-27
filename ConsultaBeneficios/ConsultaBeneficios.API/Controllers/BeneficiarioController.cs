using ConsultaBeneficios.API.Interfaces;
using ConsultaBeneficios.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaBeneficios.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BeneficiarioController : ControllerBase
    {
        private readonly IBeneficiarioServices _beneficiarioServices;

        public BeneficiarioController(IBeneficiarioServices beneficiarioServices)
        {
            _beneficiarioServices = beneficiarioServices;
        }


        /// <summary>
        /// Permite consultar os dados de um beneficiário através do CPF.
        /// </summary>
        /// <param name="cpf">CPF do Beneficiário</param>
        /// <returns>Dados do beneficiário e a listagem com o número de matrícula e código dos benefícios.</returns>
        [HttpGet(Name = "ConsultarBeneficiario")]
        [ProducesResponseType(typeof(GenericResponse<Beneficiario>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConsultarBeneficiario(string cpf)
        {
            try
            {
                var beneficiario = await _beneficiarioServices.ConsultarBeneficiarioAsync(cpf);
                var response = new GenericResponse<Beneficiario> { Dados = beneficiario };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse { Observacoes = ex.Message });
            }
        }
    }
}
