using API.Microservice.Modelo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Microservice.Contratos.Repositorio
{
    public interface IProductoRepositorio
    {
        Task<bool> Create(Producto producto);
        Task<bool> Update(Producto producto);
        Task<bool> Delete(string partitionKey, string rowKey);
        Task<List<Producto>> GetAll();
        Task<Producto> Get(string rowKey);
    }
}
