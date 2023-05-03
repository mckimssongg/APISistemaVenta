using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;


namespace SistemaVenta.BLL.Servicios
{
    public class VentaService: IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IMapper _mapper;

        public VentaService(
            IVentaRepository ventaRepository,
            IGenericRepository<DetalleVenta> detalleVentaRepositorio,
            IMapper mapper
        ){
            _ventaRepository = ventaRepository;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepository.Consultar();
            var listarResultado = new List<Venta>();

            try {
                if (buscarPor == "fecha")
                {
                    DateTime fetch_incio = DateTime.ParseExact(
                        fechaInicio,
                        "dd/MM/yyyy",
                        new CultureInfo("es-GT")
                        );
                    DateTime fetch_fin = DateTime.ParseExact(
                        fechaFin,
                        "dd/MM/yyyy",
                        new CultureInfo("es-GT")
                        );

                    listarResultado = await query.Where(v =>
                        v.FechaRegistro.Value.Date >= fetch_incio.Date &&
                        v.FechaRegistro.Value.Date <= fetch_fin.Date
                        ).Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation)
                        .ToListAsync();
                }
                else {

                    listarResultado = await query.Where(v =>
                        v.NumeroDocumento == numeroVenta
                        ).Include(dv => dv.DetalleVenta)
                        .ThenInclude(p => p.IdProductoNavigation)
                        .ToListAsync();

                }

            } catch { throw; }

            return _mapper.Map<List<VentaDTO>>(listarResultado);
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try {
                var ventaGenerada = await _ventaRepository.Registrar(_mapper.Map<Venta>(modelo));
                if(ventaGenerada.IdVenta == 0) throw new Exception("No se pudo registrar la venta");

                return _mapper.Map<VentaDTO>(ventaGenerada);

            } catch { throw; }
        }

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
            var listaResultado = new List<DetalleVenta>();

            try {
                DateTime fetch_incio = DateTime.ParseExact(
                       fechaInicio,
                       "dd/MM/yyyy",
                       new CultureInfo("es-GT")
                       );
                DateTime fetch_fin = DateTime.ParseExact(
                    fechaFin,
                    "dd/MM/yyyy",
                    new CultureInfo("es-GT")
                    );

                listaResultado = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv =>
                        dv.IdProductoNavigation.FechaRegistro.Value.Date >= fetch_incio.Date &&
                        dv.IdProductoNavigation.FechaRegistro.Value.Date <= fetch_fin.Date
                    )
                    .ToListAsync();
            } catch { throw; }

            return _mapper.Map<List<ReporteDTO>>(listaResultado);
        }
    }
}
