using ConsultaBeneficios.API.Models;

namespace ConsultaBeneficios.API.Interfaces
{
    public interface IElasticServices
    {
        Task CreateIndexIfNotExistsAsync();
        Task<bool> AddOrUpdate(Beneficiario beneficiario);
        Task<Beneficiario> Get(string key);
        Task<List<Beneficiario>?> GetAll();
        Task<bool> Remove(string key);
    }
}
