using API.Microservice.Contratos.Repositorio;
using API.Microservice.Modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace API.Microservice.EndPoints
{
    public class ProveedorFunction
    {
        private readonly ILogger<ProveedorFunction> _logger;
        private readonly IProveedorRepositorio repositorio;

        public ProveedorFunction(ILogger<ProveedorFunction> logger, IProveedorRepositorio repositorio)
        {
            _logger = logger;
            this.repositorio = repositorio;
        }

        [Function("InsertarProveedor")]
        [OpenApiOperation("Insertarspec", "InsertarProveedor", Description = " Sirve para ingresar una Proveedor")]
        [OpenApiRequestBody("application/json", typeof(Proveedor),
            Description = "Ingresar Proveedor nueva")]
        public async Task<HttpResponseData> InsertarProveedor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            HttpResponseData response;
            try
            {
                var proveedor = await req.ReadFromJsonAsync<Proveedor>() ?? throw new Exception("Debe ingresar un proveedor con todos sus datos");
                proveedor.RowKey = Guid.NewGuid().ToString();
                proveedor.Timestamp = DateTimeOffset.UtcNow;
                bool success = await repositorio.Create(proveedor);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("ListarProveedores")]
        [OpenApiOperation("Listarspec", "ListarProveedor", Description = " Sirve para listar todas los proveedores")]

        public async Task<HttpResponseData> ListarProveedores(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            HttpResponseData response;
            try
            {
                var listaProveedores = await repositorio.GetAll();
                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(listaProveedores);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("EditarProveedor")]
        [OpenApiOperation("Modificarspec", "ModificarProveedor", Description = " Sirve para editar una Proveedor")]
        [OpenApiRequestBody("application/json", typeof(Proveedor),
            Description = "editar Proveedor")]
        public async Task<HttpResponseData> EditarProveedor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "EditarProveedor/{RowKey}")] HttpRequestData req,
            string rowKey)
        {
            HttpResponseData response;
            try
            {
                var proveedor = await req.ReadFromJsonAsync<Proveedor>() ?? throw new Exception("Debe ingresar un proveedor con todos sus datos");
                proveedor.RowKey = rowKey;
                bool success = await repositorio.Update(proveedor);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("BorrarProveedor")]
        [OpenApiOperation("Eliminarspec", "DeleteProveedor", Description = " Sirve para eliminar una Proveedor")]
        [OpenApiParameter(name: "partitionKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "PartitionKey de la Proveedor", Description = "El PartitionKey de la Proveedor a borrar", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "rowkey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "RowKey de la Proveedor", Description = "El RowKey de la Proveedor a borrar", Visibility = OpenApiVisibilityType.Important)]

        public async Task<HttpResponseData> BorrarProveedor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "BorrarProveedor/{partitionKey}/{rowKey}")] HttpRequestData req,
            string partitionKey, string rowKey)
        {
            HttpResponseData response;
            try
            {
                bool success = await repositorio.Delete(partitionKey, rowKey);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("ListarProveedorById")]
        [OpenApiOperation("Listaridspec", "ListarProveedorId", Description = " Sirve para listar una Proveedor por id")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "ID de la Proveedor", Description = "El RowKey de la Proveedor a obtener", Visibility = OpenApiVisibilityType.Important)]

        public async Task<HttpResponseData> ListarProveedorById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListarProveedorById/{RowKey}")] HttpRequestData req,
            string rowKey)
        {
            HttpResponseData response;
            try
            {
                var proveedor = await repositorio.Get(rowKey);
                response = req.CreateResponse(proveedor != null ? HttpStatusCode.OK : HttpStatusCode.NotFound);
                if (proveedor != null)
                {
                    await response.WriteAsJsonAsync(proveedor);
                }
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
