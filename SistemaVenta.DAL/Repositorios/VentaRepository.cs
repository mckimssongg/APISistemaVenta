using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBcontext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbVentaContext _dbcontext;

        public VentaRepository(DbVentaContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Venta> Registrar(Venta model)
        {
            Venta ventaGenerada = new Venta();

            // Inicio de la transacción: si dentro de la logica ocurre un error se restablece
            using var transaction = _dbcontext.Database.BeginTransaction();
            try
            {
                foreach (DetalleVenta dv in model.DetalleVenta)
                {
                    Producto producto_encontrado = _dbcontext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                    producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                    _dbcontext.Productos.Update(producto_encontrado);
                }

                await _dbcontext.SaveChangesAsync();

                NumeroDocumento correlativo = _dbcontext.NumeroDocumentos.First();

                correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                correlativo.FechaRegistro = DateTime.Now;

                _dbcontext.NumeroDocumentos.Update(correlativo);
                await _dbcontext.SaveChangesAsync();

                int CantidadDigitos = 4;
                string ceros = string.Concat(Enumerable.Repeat("0", CantidadDigitos));
                string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                numeroVenta = numeroVenta.Substring(numeroVenta.Length - CantidadDigitos, CantidadDigitos);

                model.NumeroDocumento = numeroVenta;

                await _dbcontext.Venta.AddAsync(model);
                await _dbcontext.SaveChangesAsync();

                ventaGenerada = model;

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            return ventaGenerada;
        }
    }
}
