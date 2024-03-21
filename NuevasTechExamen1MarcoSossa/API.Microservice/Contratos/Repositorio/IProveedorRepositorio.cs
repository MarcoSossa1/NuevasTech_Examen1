using API.Microservice.Modelo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Microservice.Contratos.Repositorio
{
    public interface IProveedorRepositorio
    {
        Task<bool> Create(Proveedor proveedor);
        Task<bool> Update(Proveedor proveedor);
        Task<bool> Delete(string partitionKey, string rowKey);
        Task<List<Proveedor>> GetAll();
        Task<Proveedor> Get(string id);
    }
}
