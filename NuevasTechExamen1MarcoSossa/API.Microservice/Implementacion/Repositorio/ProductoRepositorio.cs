using Azure.Data.Tables;
using API.Microservice.Contratos.Repositorio;
using API.Microservice.Modelo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Microservice.Implementacion.Repositorio
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly string? cadenaConexion;
        private readonly string tablaNombre;
        private readonly IConfiguration configuration;

        public ProductoRepositorio(IConfiguration conf)
        {
            configuration = conf;
            cadenaConexion = configuration.GetSection("cadenaconexion").Value;
            tablaNombre = "Producto";
        }

        public async Task<bool> Create(Producto producto)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpsertEntityAsync(producto);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(string partitionKey, string rowKey)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.DeleteEntityAsync(partitionKey, rowKey);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Producto> Get(string rowKey)
        {
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Productos' and RowKey eq '{rowKey}'";
            await foreach (Producto producto in tablaCliente.QueryAsync<Producto>(filter: filtro))
            {
                return producto;
            }
            return null;
        }

        public async Task<List<Producto>> GetAll()
        {
            List<Producto> lista = new List<Producto>();
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Productos'";

            await foreach (Producto producto in tablaCliente.QueryAsync<Producto>(filter: filtro))
            {
                lista.Add(producto);
            }

            return lista;
        }

        public async Task<bool> Update(Producto producto)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpdateEntityAsync(producto, producto.ETag);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
