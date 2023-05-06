using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Servicios;


namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> Listar()
        {
            var rsp = new Response<List<CategoriaDTO>>();

            try
            {
                rsp.status = true;
                rsp.value = await _categoriaService.Lista();
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.message = ex.Message;
            }

            return Ok(rsp);
        }
    }
}
