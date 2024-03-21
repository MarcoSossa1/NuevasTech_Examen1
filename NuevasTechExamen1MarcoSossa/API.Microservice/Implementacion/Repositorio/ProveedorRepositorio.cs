using Azure.Data.Tables;
using API.Microservice.Contratos.Repositorio;
using API.Microservice.Modelo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Microservice.Implementacion.Repositorio
{
    public class ProveedorRepositorio : IProveedorRepositorio
    {
        private readonly string? cadenaConexion;
        private readonly string tablaNombre;
        private readonly IConfiguration configuration;

        public ProveedorRepositorio(IConfiguration conf)
        {
            configuration = conf;
            cadenaConexion = configuration.GetSection("cadenaconexion").Value;
            tablaNombre = "Proveedor";
        }

        public async Task<bool> Create(Proveedor proveedor)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpsertEntityAsync(proveedor);
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

        public async Task<Proveedor> Get(string id)
        {
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Proveedores' and RowKey eq '{id}'";
            await foreach (Proveedor proveedor in tablaCliente.QueryAsync<Proveedor>(filter: filtro))
            {
                return proveedor;
            }
            return null;
        }

        public async Task<List<Proveedor>> GetAll()
        {
            List<Proveedor> lista = new List<Proveedor>();
            var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
            var filtro = $"PartitionKey eq 'Proveedores'";

            await foreach (Proveedor proveedor in tablaCliente.QueryAsync<Proveedor>(filter: filtro))
            {
                lista.Add(proveedor);
            }

            return lista;
        }

        public async Task<bool> Update(Proveedor proveedor)
        {
            try
            {
                var tablaCliente = new TableClient(cadenaConexion, tablaNombre);
                await tablaCliente.UpdateEntityAsync(proveedor, proveedor.ETag);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
