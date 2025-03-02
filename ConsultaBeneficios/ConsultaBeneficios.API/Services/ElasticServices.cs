using ConsultaBeneficios.API.Interfaces;
using ConsultaBeneficios.API.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http.Headers;
using System.Text;

namespace ConsultaBeneficios.API.Services
{
    public class ElasticServices : IElasticServices
    {
        private readonly ElasticsearchClient _client;
        private readonly string _defaultIndexName;

        public ElasticServices(IConfiguration configuration)
        {
            _defaultIndexName = configuration.GetValue<string>("ElasticSettings:DefaultIndex");

            var settings = new ElasticsearchClientSettings(new Uri(configuration.GetValue<string>("ElasticSettings:URL")))
             .Authentication(new BasicAuthentication(configuration.GetValue<string>("ElasticSettings:Username"), configuration.GetValue<string>("ElasticSettings:Password")))
             .DefaultIndex(_defaultIndexName);

            _client = new ElasticsearchClient(settings);
        }
        public async Task CreateIndexIfNotExistsAsync()
        {
            if (!(await _client.Indices.ExistsAsync(_defaultIndexName)).Exists)
                await _client.Indices.CreateAsync(_defaultIndexName);
        }

        public async Task<bool> AddOrUpdate(Beneficiario beneficiario)
        {
            await CreateIndexIfNotExistsAsync();

            var result = await _client.IndexAsync(beneficiario, idx => idx.Index(_defaultIndexName).Id(beneficiario.CPF).OpType(OpType.Index));

            return result.IsValidResponse;
        }

        public async Task<Beneficiario> Get(string key)
        {
            var result = await _client.GetAsync<Beneficiario>(key, idx => idx.Index(_defaultIndexName));

            if (!result.IsValidResponse)
                throw new Exception("CPF não encontrado.");

            return result.Source;
        }

        public async Task<List<Beneficiario>?> GetAll()
        {
            var result = await _client.SearchAsync<Beneficiario>(idx => idx.Index(_defaultIndexName));

            if (!result.IsValidResponse)
                throw new Exception("Nenhum registro encontrado.");

            return result.Documents.ToList();
        }

        public async Task<bool> Remove(string key)
        {
            await CreateIndexIfNotExistsAsync();

            var result = await _client.DeleteAsync<Beneficiario>(key, idx => idx.Index(_defaultIndexName));
            return result.IsValidResponse;
        }
    }
}
