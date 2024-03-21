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
    public class ProductoFunction
    {
        private readonly ILogger<ProductoFunction> _logger;
        private readonly IProductoRepositorio repositorio;

        public ProductoFunction(ILogger<ProductoFunction> logger, IProductoRepositorio repositorio)
        {
            _logger = logger;
            this.repositorio = repositorio;
        }

        [Function("InsertarProducto")]
        [OpenApiOperation("Insertarspec", "InsertarProducto", Description = "Sirve para ingresar un Producto")]
        [OpenApiRequestBody("application/json", typeof(Producto),
            Description = "Ingresar Producto nueva")]
        public async Task<HttpResponseData> InsertarProducto(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            HttpResponseData response;
            try
            {
                var producto = await req.ReadFromJsonAsync<Producto>() ?? throw new Exception("Debe ingresar un producto con todos sus datos");
                producto.RowKey = Guid.NewGuid().ToString();
                producto.Timestamp = DateTimeOffset.UtcNow;
                bool success = await repositorio.Create(producto);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("ListarProductos")]
        [OpenApiOperation("Listarspec", "ListarProducto", Description = "Sirve para listar todos los productos")]
        public async Task<HttpResponseData> ListarProductos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            HttpResponseData response;
            try
            {
                var listaProductos = await repositorio.GetAll();
                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(listaProductos);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("EditarProducto")]
        [OpenApiOperation("Modificarspec", "ModificarProducto", Description = "Sirve para editar un Producto")]
        [OpenApiRequestBody("application/json", typeof(Producto),
            Description = "Editar Producto")]
        public async Task<HttpResponseData> EditarProducto(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "EditarProducto/{id}")] HttpRequestData req,
            string rowKey)
        {
            HttpResponseData response;
            try
            {
                var producto = await req.ReadFromJsonAsync<Producto>() ?? throw new Exception("Debe ingresar un producto con todos sus datos");
                producto.RowKey = rowKey;
                bool success = await repositorio.Update(producto);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("BorrarProducto")]
        [OpenApiOperation("Eliminarspec", "EliminarProducto", Description = "Sirve para eliminar un Producto")]
        [OpenApiParameter(name: "partitionKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "PartitionKey del Producto", Description = "El PartitionKey del Producto a borrar", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "rowkey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "RowKey del Producto", Description = "El RowKey del Producto a borrar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> BorrarProducto(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "BorrarProducto/{partitionKey}/{rowKey}")] HttpRequestData req,
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

        [Function("ListarProductoById")]
        [OpenApiOperation("Listaridspec", "ListarProductoById", Description = "Sirve para listar un Producto por id")]
        [OpenApiParameter(name: "rowKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "ID de la Producto", Description = "El RowKey de la Producto a obtener", Visibility = OpenApiVisibilityType.Important)]

        public async Task<HttpResponseData> ListarProductoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListarProductoById/{rowKey}")] HttpRequestData req,
            string rowKey)
        {
            HttpResponseData response;
            try
            {
                var producto = await repositorio.Get(rowKey);
                response = req.CreateResponse(producto != null ? HttpStatusCode.OK : HttpStatusCode.NotFound);
                if (producto != null)
                {
                    await response.WriteAsJsonAsync(producto);
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
