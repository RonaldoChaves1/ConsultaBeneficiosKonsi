using ConsultaBeneficios.API.Extensions;
using ConsultaBeneficios.API.KonsiClient.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ConsultaBeneficios.API.KonsiClient
{
    public class KonsiApiClient
    {
        protected const string TOKEN_ENDPOINT = "/api/v1/token";
        protected const string PARAM_USUARIO_TOKEN_ENDPOINT = "username";
        protected const string PARAM_SENHA_TOKEN_ENDPOINT = "password";
        protected const string CONSULTAR_BENEFICIOS_ENDPOINT = "/api/v1/inss/consulta-beneficios";
        protected const string PARAM_GET_CONSULTAR_BENEFICIOS_ENDPOINT = "?cpf={0}";
        protected const string MIME_JSON = "application/json";

        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private DistributedCacheEntryOptions _cacheOption;
        private readonly string _usuario;
        private readonly string _senha;

        private TokenResponse _token = new();

        public KonsiApiClient(string url, string usuario, string senha, IDistributedCache cache)
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri(url) };
            _usuario = usuario;
            _senha = senha;
            _cache = cache;
            _cacheOption = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) };
        }

        public async Task<BeneficiarioResponse> ConsultarBeneficio(string cpf)
        {
            var responseText = await _cache.GetStringAsync($"beneficiario-{cpf}");

            if (string.IsNullOrWhiteSpace(responseText))
            {
                if (!_token.Valido)
                    await DefinirTokenAsync();

                var parametros = string.Format(PARAM_GET_CONSULTAR_BENEFICIOS_ENDPOINT, cpf);

                var response = await _httpClient.GetAsync(string.Format("{0}{1}", CONSULTAR_BENEFICIOS_ENDPOINT, parametros));
                responseText = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(responseText))
                    await _cache.SetStringAsync($"beneficiario-{cpf}", responseText, _cacheOption);
            }

            var resultado = await ConverterApiResponseAsync<GenericResponse<BeneficiarioResponse>>(responseText);

            if (resultado != null && resultado.Sucesso)
            {
                return resultado.Dados;
            }

            throw new Exception(resultado.Observacoes);
        }

        private async Task DefinirTokenAsync()
        {
            var credenciais = new Dictionary<string, string>
            {
                { PARAM_USUARIO_TOKEN_ENDPOINT, _usuario },
                { PARAM_SENHA_TOKEN_ENDPOINT, _senha }
            };

            _token = await GetTokenAsync(credenciais);

            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MIME_JSON));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"{_token.Tipo} {_token.Valor}");
        }

        private async Task<TokenResponse> GetTokenAsync(Dictionary<string, string> credenciais)
        {
            var credenciaisJson = JsonSerializer.Serialize(credenciais);

            using (var content = new StringContent(credenciaisJson, Encoding.UTF8, MIME_JSON))
            {
                var response = await (await _httpClient.PostAsync(TOKEN_ENDPOINT, content)).Content.ReadAsStringAsync();

                var resultado = await ConverterApiResponseAsync<GenericResponse<TokenResponse>>(response);

                if (resultado != null && resultado.Sucesso)
                {
                    return resultado.Dados;
                }

                throw new Exception(resultado.Observacoes);
            }
        }

        protected async Task<T> ConverterApiResponseAsync<T>(string responseText)
        {
            T apiResponse;

            try
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new CustomDateTimeConverter());

                apiResponse = JsonSerializer.Deserialize<T>(responseText, options);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Resposta inválida:{0}", responseText));
            }

            return apiResponse;
        }
    }
}
