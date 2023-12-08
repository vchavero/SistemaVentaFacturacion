using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IRegimenFiscalService _regimenFiscalService;
        private readonly IMapper _mapper;
        public ClienteController(
            IClienteService clienteService,
            IRegimenFiscalService regimenFiscalService,
            IMapper mapper
            )
        {
            this._clienteService = clienteService;
            this._regimenFiscalService = regimenFiscalService;
            this._mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();
            try
            {
                VMCliente vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);

                Cliente cliente_creado = await this._clienteService.Crear(_mapper.Map<Cliente>(vmCliente));

                vmCliente = _mapper.Map<VMCliente>(cliente_creado);

                gResponse.Estado = true;
                gResponse.Objeto = vmCliente;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCliente> listaCliente = this._mapper.Map<List<VMCliente>>(await this._clienteService.ListarClientes());
            return StatusCode(StatusCodes.Status200OK, listaCliente);
        }

        [HttpGet]
        public async Task<IActionResult> DetallesPorId(int idCliente)
        {
            GenericResponse<VMClienteDetallado> gResponse = new GenericResponse<VMClienteDetallado>();
            try
            {
                Cliente cliente = await this._clienteService.ObtenerPorId(idCliente);
                RegimenFiscal regimen = (await this._regimenFiscalService.Lista()).FirstOrDefault(x => x.cveRegimen == cliente.cveRegimen, null);
                VMClienteDetallado clienteDetallado = new VMClienteDetallado() {
                    idCliente = cliente.idCliente,
                    nombre = cliente.nombre,
                    rfc = cliente.rfc,
                    codigo_postal = cliente.codigo_postal,
                    cveRegimen = cliente.cveRegimen,
                    regimen = (regimen != null) ? regimen.descripcion : ""
                };
                gResponse.Estado = true;
                gResponse.Objeto = clienteDetallado;    

            } catch(Exception e)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = e.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();
            try
            {

                VMCliente vmCliente = JsonConvert.DeserializeObject<VMCliente>(modelo);
                Cliente clienteEditado = await this._clienteService.Editar(_mapper.Map<Cliente>(vmCliente));
                vmCliente = _mapper.Map<VMCliente>(clienteEditado);
                gResponse.Estado = true;
                gResponse.Objeto = vmCliente;

            } catch(Exception e)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = e.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idCliente)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _clienteService.Eliminar(idCliente);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
