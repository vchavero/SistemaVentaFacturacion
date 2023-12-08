using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Drawing;
using System.Xml;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaServicio;
        
        private readonly IVentaService _ventaServicio;
        private readonly IFormaPagoService _formaPagoServicio;
        private readonly IUsoCFDIService _usoCFDIService;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;

        public VentaController(
            ITipoDocumentoVentaService tipoDocumentoVentaServicio,
            IVentaService ventaServicio,
            IFormaPagoService formaPagoService,
            IUsoCFDIService usoCFDIService,
            IMapper mapper,
             IConverter converter
            )
        {
            
            _tipoDocumentoVentaServicio = tipoDocumentoVentaServicio;
            _ventaServicio = ventaServicio;
            _formaPagoServicio = formaPagoService;
            _usoCFDIService = usoCFDIService;
            _mapper = mapper;
            _converter = converter;
        }

        public IActionResult NuevaVenta()
        {
            return View();
        }

        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {

            List<VMTipoDocumentoVenta> vmListaTipoDocumentos = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaServicio.Lista());

            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentos);
        }

        [HttpGet]
        public async Task<IActionResult> ListaFormaPago()
        {
            List<VMFormaPago> formasDePago = _mapper.Map<List<VMFormaPago>>(await _formaPagoServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, formasDePago);
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsoCFDI()
        {
            List<VMUsoCFDI> usosCFDI = _mapper.Map<List<VMUsoCFDI>>(await _usoCFDIService.Lista());
            return StatusCode(StatusCodes.Status200OK, usosCFDI);
        }

        [HttpGet]
        public async Task<IActionResult> ListaMetodoPago()
        {
            List<VMMetodoPago> metodosDePago = new List<VMMetodoPago>();
                metodosDePago.Add(new VMMetodoPago("PUE", "Pago en Una sola Exhibición"));
                metodosDePago.Add(new VMMetodoPago("PPD", "Pago en Parcialidades o Diferido"));
            
            return StatusCode(StatusCodes.Status200OK, metodosDePago);
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaServicio.ObtenerProductos(busqueda));

            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }



        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
        {

            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                modelo.IdUsuario = int.Parse(idUsuario);

                if (modelo.IdTipoDocumentoVenta == 2 && modelo.IdCliente == 0) throw new TaskCanceledException("Se debe seleccionar cliente al facturar");

                Venta venta_creada = await _ventaServicio.Registrar(_mapper.Map<Venta>(modelo));
                modelo = _mapper.Map<VMVenta>(venta_creada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;

            }
            catch(Exception ex) {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }



        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta,string fechaInicio, string fechaFin)
        {

            List<VMDetallesFacturaVenta> vmHistorialVenta = _mapper.Map<List<VMDetallesFacturaVenta>>(
                            _mapper.Map<List<VMVenta>>(await _ventaServicio.Historial(numeroVenta, fechaInicio, fechaFin))
                            );


            // A LAS VENTAS DONNDE SE SOLICITA FACTURA, LE ENVIAMOS SUS DATOS
            foreach (VMDetallesFacturaVenta venta in vmHistorialVenta.Where(x => x.IdTipoDocumentoVenta == 2))
            {
                DetallesFacturaVenta ventaDetalles = await this._ventaServicio.GetDetallesFactura(_mapper.Map<DetallesFacturaVenta>(venta));
                venta.NombreCliente = ventaDetalles.NombreCliente;
                venta.RFC = ventaDetalles.RFC;
                venta.CodigoPostal = ventaDetalles.CodigoPostal;
                venta.CveRegimen = ventaDetalles.CveRegimen;
                venta.Regimen = ventaDetalles.Regimen;
                venta.FormaPago = ventaDetalles.FormaPago;
                venta.UsoCFDI = ventaDetalles.UsoCFDI;
                venta.MetodoPago = ventaDetalles.MetodoPago;
            }

            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }


        public async Task<IActionResult> MostrarPDFVenta(string numeroVenta) {

            Venta venta = await this._ventaServicio.Detalle(numeroVenta);

            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/";
            if(venta.IdTipoDocumentoVenta == 2)
            {
                urlPlantillaVista += $"PDFVentaFactura?numeroVenta={numeroVenta}";
            } else
            {
                urlPlantillaVista += $"PDFVenta?numeroVenta={numeroVenta}";
            }

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings() { 
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects = { 
                    new ObjectSettings(){ 
                        Page = urlPlantillaVista
                    }
                }
            };

            var archivoPDF = _converter.Convert(pdf);

            return File(archivoPDF, "application/pdf");

        }

    }
}
