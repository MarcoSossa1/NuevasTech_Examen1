using Azure;
using Azure.Data.Tables;
using Shared.Interfaces;
using System;

namespace API.Microservice.Modelo
{
    public class Producto : IProducto, ITableEntity
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public string ProveedorId { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
