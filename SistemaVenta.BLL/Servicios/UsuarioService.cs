using System;
using System.Collections.Generic;
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
    public class UsuarioService: IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepositorio;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO usuario)
        {
            try { 
                var usuarioCreado = await _usuarioRepositorio.Crear(_mapper.Map<Usuario>(usuario));
                if (usuarioCreado.IdUsuario == 0)
                {
                    throw new TaskCanceledException("El usuario no se pudo crear");
                }

                var queryUserCreado = await _usuarioRepositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);

                usuarioCreado = queryUserCreado.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch { throw; }
        }

        public async Task<bool> Editar(UsuarioDTO usuario)
        {
            try {
                var usuarioModelo = _mapper.Map<Usuario>(usuario);

                var usuarioEncontrado = await _usuarioRepositorio.Obtener(u => u.IdUsuario == usuarioModelo.IdUsuario);

                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");

                }

                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;

                bool respuesta = await _usuarioRepositorio.Editar(usuarioEncontrado);

                if (!respuesta) {
                    throw new TaskCanceledException("No se pudo editar :(");
                }
                return respuesta;

            } catch { throw; }
        }

        public async Task<bool> Eliminar(int id)
        {
            try {
                var usuarioEncontrado  = await _usuarioRepositorio.Obtener(u => u.IdUsuario == id);

                if (usuarioEncontrado == null) {
                    throw new TaskCanceledException("No existe el usuario");
                }

                bool respuesta = await _usuarioRepositorio.Eliminar(usuarioEncontrado);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo eliminar el usuario");

                }

                return respuesta;

            } catch {

                throw;

            }
        }

        public async Task<List<UsuarioDTO>> Lista()
        {
            try {
                var queryUsuario = await _usuarioRepositorio.Consultar();
                var listaUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();

                return _mapper.Map<List<UsuarioDTO>>(listaUsuario);

            } catch {
                throw;
            }
        }

        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuario = await _usuarioRepositorio.Consultar(u =>
                    u.Correo == correo &&
                    u.Clave == clave
                    );
                if (queryUsuario.FirstOrDefault() == null)
                    throw new TaskCanceledException("El usuario no existe");

                Usuario devolverUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<SesionDTO>(devolverUsuario);

            }
            catch { throw; }
        }
    }
}
