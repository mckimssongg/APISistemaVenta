using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DTO
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public int? IdCategoria { get; set; }
        public string? CategoriaDescripcion { get; set; }
        public int? Stock { get; set; }
        public string? PrecioTexto { get; set; } // TODO: Change to decimal
        public int? EsActivo { get; set; } 
    }
}
