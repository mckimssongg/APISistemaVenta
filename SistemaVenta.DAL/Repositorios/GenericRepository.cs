using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.DBcontext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Repositorios
{
    public class GenericRepository<Tmodelo> : IGenericRepository<Tmodelo> where Tmodelo : class
    {
        private readonly DbVentaContext _dbcontext;

        public GenericRepository(DbVentaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Tmodelo> Obtener(Expression<Func<Tmodelo, bool>> filtro)
        {
            try
            {
                Tmodelo tmodelo = await _dbcontext.Set<Tmodelo>().FirstOrDefaultAsync(filtro);
                return tmodelo;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tmodelo> Crear(Tmodelo model)
        {
            try
            {
                _dbcontext.Set<Tmodelo>().Add(model);
                await _dbcontext.SaveChangesAsync();
                return model;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(Tmodelo model)
        {
            try
            {
                _dbcontext.Set<Tmodelo>().Update(model);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(Tmodelo model)
        {
            try
            {
                _dbcontext.Set<Tmodelo>().Remove(model);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IQueryable<Tmodelo>> Consultar(Expression<Func<Tmodelo, bool>> filtro = null)
        {
            try
            {
                IQueryable<Tmodelo> queryModelo = filtro == null ? _dbcontext.Set<Tmodelo>() : _dbcontext.Set<Tmodelo>().Where(filtro);
                return queryModelo;
            }
            catch
            {
                throw;
            }
        }

    }
}
