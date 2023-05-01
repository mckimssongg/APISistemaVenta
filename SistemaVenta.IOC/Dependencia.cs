﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBcontext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.Repositorios;

using SistemaVenta.Utility;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<DbVentaContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("cadenaSQL"));
            });

            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            service.AddScoped<IVentaRepository, VentaRepository>();

            service.AddAutoMapper(typeof(AutoMapperProfile));
        }
    }
}
