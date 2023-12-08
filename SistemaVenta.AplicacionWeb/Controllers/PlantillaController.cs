using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Xml;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
using Microsoft.CodeAnalysis;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioServicio;
        private readonly IVentaService _ventaServicio;
        private readonly IHostingEnvironment _environment;

        public PlantillaController(
            IHostingEnvironment hostEnvironment,
            IMapper mapper,
            INegocioService negocioServicio,
            IVentaService ventaServicio)
        {
            _environment = hostEnvironment;
            _mapper = mapper;
            _negocioServicio = negocioServicio;
            _ventaServicio = ventaServicio;
        }
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }

        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            
            VMDetallesFacturaVenta venta = _mapper.Map<VMDetallesFacturaVenta>( _mapper.Map<VMVenta>( await _ventaServicio.Detalle(numeroVenta) ) );
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.negocio = vmNegocio;
            modelo.venta = venta;

            return View(modelo);
        }

        public async Task<IActionResult> PDFVentaFactura(string numeroVenta)
        {

            VMDetallesFacturaVenta venta = _mapper.Map<VMDetallesFacturaVenta>(_mapper.Map<VMVenta>(await _ventaServicio.Detalle(numeroVenta)));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.negocio = vmNegocio;
            modelo.venta = venta;

            DetallesFacturaVenta detalleFactura = await this._ventaServicio.GetDetallesFactura(_mapper.Map<DetallesFacturaVenta>(venta));

            venta.NombreCliente = detalleFactura.NombreCliente;
            venta.RFC = detalleFactura.RFC;
            venta.CodigoPostal = detalleFactura.CodigoPostal;
            venta.CveRegimen = detalleFactura.CveRegimen;
            venta.Regimen = detalleFactura.Regimen;
            venta.FormaPago = detalleFactura.FormaPago;
            venta.UsoCFDI = detalleFactura.UsoCFDI;
            venta.MetodoPago = detalleFactura.MetodoPago;
            modelo.venta = venta;

            //this.generateXML(venta, vmNegocio);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> createXML(string numeroVenta)
        {
            VMDetallesFacturaVenta venta = _mapper.Map<VMDetallesFacturaVenta>(_mapper.Map<VMVenta>(await _ventaServicio.Detalle(numeroVenta)));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());

            DetallesFacturaVenta detalleFactura = await this._ventaServicio.GetDetallesFactura(_mapper.Map<DetallesFacturaVenta>(venta));

            venta.NombreCliente = detalleFactura.NombreCliente;
            venta.RFC = detalleFactura.RFC;
            venta.CodigoPostal = detalleFactura.CodigoPostal;
            venta.CveRegimen = detalleFactura.CveRegimen;
            venta.FormaPago = detalleFactura.FormaPago;
            venta.UsoCFDI = detalleFactura.UsoCFDI;
            venta.MetodoPago = detalleFactura.MetodoPago;

            //string filename = "File.pdf";
            this.generateXML(venta, vmNegocio);

            return StatusCode(StatusCodes.Status200OK);
        }


        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }

        private string generateXML(VMDetallesFacturaVenta datosFactura, VMNegocio negocio)
        {
            string nombreXML = _environment.ContentRootPath + "/wwwroot/facturas/" + "factura_" + datosFactura.IdVenta + ".xml";
            XmlDocument xml = new XmlDocument();

            XmlNode root = xml.CreateElement("cfdi:Comprobante");
            XmlAttribute version = xml.CreateAttribute("Version");
            version.Value = "4";
            root.Attributes.Append(version);
            XmlAttribute fecha = xml.CreateAttribute("Fecha");
            fecha.Value = DateTime.Parse(datosFactura.FechaRegistro).ToString("yyyy-MM-ddTHH:mm:ss");
            root.Attributes.Append(fecha);
            XmlAttribute serie = xml.CreateAttribute("Serie");
            serie.Value = "CFDI4.0";
            root.Attributes.Append(serie);
            XmlAttribute formaPago = xml.CreateAttribute("FormaPago");
            formaPago.Value = datosFactura.CveFormaPago.HasValue ? string.Format("{0:00}", datosFactura.CveFormaPago) : "";
            root.Attributes.Append(formaPago);
            XmlAttribute moneda = xml.CreateAttribute("Moneda");
            moneda.Value = "MXN";
            root.Attributes.Append(moneda);
            XmlAttribute subTotal = xml.CreateAttribute("SubTotal");
            subTotal.Value = datosFactura.SubTotal;
            root.Attributes.Append(subTotal);
            XmlAttribute total = xml.CreateAttribute("Total");
            total.Value = datosFactura.Total;
            root.Attributes.Append(total);
            XmlAttribute tipoComprobante = xml.CreateAttribute("TipoDeComprobante");
            tipoComprobante.Value = "I";
            root.Attributes.Append(tipoComprobante);
            XmlAttribute metodoPago = xml.CreateAttribute("MetodoPago");
            metodoPago.Value = datosFactura.CveMetodoPago;
            root.Attributes.Append(metodoPago);
            XmlAttribute folio = xml.CreateAttribute("folio");
            folio.Value = datosFactura.NumeroVenta;
            root.Attributes.Append(folio);
            xml.AppendChild(root);

            #region Emisor
            XmlNode emisorNode = xml.CreateElement("cfdi:Emisor");
            XmlAttribute rfcEmisor = xml.CreateAttribute("Rfc");
            rfcEmisor.Value = "X100AA1023890";
            emisorNode.Attributes.Append(rfcEmisor);
            XmlAttribute nombreEmisor = xml.CreateAttribute("Nombre");
            nombreEmisor.Value = negocio.Nombre;
            emisorNode.Attributes.Append(nombreEmisor);
            XmlAttribute regimenEmisor = xml.CreateAttribute("RegimenFiscal");
            regimenEmisor.Value = "601";
            emisorNode.Attributes.Append(regimenEmisor);
            root.AppendChild(emisorNode);
            #endregion

            #region Receptor
            XmlNode receptorNode = xml.CreateElement("cfdi:Receptor");
            XmlAttribute rfcReceptor = xml.CreateAttribute("Rfc");
            rfcReceptor.Value = datosFactura.RFC;
            receptorNode.Attributes.Append(rfcReceptor);
            XmlAttribute nombreReceptor = xml.CreateAttribute("Nombre");
            nombreReceptor.Value = datosFactura.NombreCliente;
            receptorNode.Attributes.Append(nombreReceptor);
            XmlAttribute domicilioReceptor = xml.CreateAttribute("DomicilioFiscalReceptor");
            domicilioReceptor.Value = datosFactura.CodigoPostal;
            receptorNode.Attributes.Append(domicilioReceptor);
            XmlAttribute regimenReceptor = xml.CreateAttribute("RegimenFiscalReceptor");
            regimenReceptor.Value = datosFactura.CveRegimen.ToString();
            receptorNode.Attributes.Append(regimenReceptor);
            XmlAttribute usoCfdiReceptor = xml.CreateAttribute("UsoCFDI");
            usoCfdiReceptor.Value = datosFactura.CveUsoCFDI;
            receptorNode.Attributes.Append(usoCfdiReceptor);
            root.AppendChild(receptorNode);
            #endregion

            #region Conceptos

            XmlNode conceptosNode = xml.CreateElement("cfdi:Conceptos");
            XmlNode concepto = xml.CreateElement("cfdi:Concepto");
            XmlAttribute cantidad;
            XmlAttribute noIdentificacion;
            XmlAttribute descripcion;
            XmlAttribute valorUnitario;
            double precioProducto;
            double importeProducto;
            XmlAttribute importe;
            XmlAttribute objetoImp = xml.CreateAttribute("ObjetoImp");
            objetoImp.Value = "02";

            XmlNode Impuestos = xml.CreateElement("cfdi:Impuestos");
            XmlAttribute TotalImpuestosTrasladados;
            XmlNode Traslados = xml.CreateElement("cfdi:Traslados");
            XmlNode Traslado = xml.CreateElement("cfdi:Traslado");
            XmlAttribute Base;
            XmlAttribute Impuesto = xml.CreateAttribute("Impuesto");
            Impuesto.Value = "002";
            XmlAttribute tipoFactor = xml.CreateAttribute("TipoFactor");
            tipoFactor.Value = "Tasa";
            XmlAttribute tasaOCuota = xml.CreateAttribute("TasaOCuota");
            tasaOCuota.Value = "0.160000";
            double importeIVA;

            foreach (VMDetalleVenta venta in datosFactura.DetalleVenta)
            {
                concepto = xml.CreateElement("cfdi:Concepto");
                cantidad = xml.CreateAttribute("Cantidad");
                cantidad.Value = venta.Cantidad.ToString();
                concepto.Attributes.Append(cantidad);
                noIdentificacion = xml.CreateAttribute("NoIdentificacion");
                noIdentificacion.Value = venta.IdProducto.ToString();
                concepto.Attributes.Append(noIdentificacion);
                descripcion = xml.CreateAttribute("Descripcion");
                descripcion.Value = venta.DescripcionProducto;
                concepto.Attributes.Append(descripcion);
                valorUnitario = xml.CreateAttribute("ValorUnitario");
                // QUITAR EL IVA
                precioProducto = double.Parse(venta.Precio) / 1.16;
                valorUnitario.Value = precioProducto.ToString();
                concepto.Attributes.Append(valorUnitario);
                importe = xml.CreateAttribute("Importe");
                importeProducto = (double)(precioProducto * venta.Cantidad);
                importe.Value = importeProducto.ToString();
                concepto.Attributes.Append(importe);
                concepto.Attributes.Append(objetoImp);

                Impuestos = xml.CreateElement("cfdi:Impuestos");
                Traslados = xml.CreateElement("cfdi:Traslados");
                Traslado = xml.CreateElement("cfdi:Traslado");

                // Traslado -> Traslados -> Impuestos -> Concepto -> Conceptos
                Base = xml.CreateAttribute("Base");
                Base.Value = importeProducto.ToString();
                Traslado.Attributes.Append(Base);
                Traslado.Attributes.Append(Impuesto);
                Traslado.Attributes.Append(tipoFactor);
                Traslado.Attributes.Append(tasaOCuota);
                importe = xml.CreateAttribute("Importe");
                importeIVA = importeProducto * 0.16;
                importe.Value = importeIVA.ToString();
                Traslado.Attributes.Append(importe);
                Traslados.AppendChild(Traslado);
                Impuestos.AppendChild(Traslados);
                concepto.AppendChild(Impuestos);
                conceptosNode.AppendChild(concepto);
            }
            root.AppendChild(conceptosNode);

            #endregion

            #region Impuesto

            Impuestos = xml.CreateElement("cfdi:Impuestos");
            TotalImpuestosTrasladados = xml.CreateAttribute("TotalImpuestosTrasladados");
            TotalImpuestosTrasladados.Value = datosFactura.ImpuestoTotal;
            Impuestos.Attributes.Append(TotalImpuestosTrasladados);
            Traslados = xml.CreateElement("cfdi:Traslados");
            Traslado = xml.CreateElement("cfdi:Traslado");
            Base = xml.CreateAttribute("Base");
            Base.Value = datosFactura.SubTotal;
            Traslado.Attributes.Append(Base);
            Traslado.Attributes.Append(Impuesto);
            Traslado.Attributes.Append(tasaOCuota);
            Traslado.Attributes.Append(tipoFactor);
            importe = xml.CreateAttribute("Importe");
            importe.Value = datosFactura.ImpuestoTotal;
            Traslado.Attributes.Append(importe);
            Traslados.AppendChild(Traslado);
            Impuestos.AppendChild(Traslados);
            root.AppendChild(Impuestos);

            #endregion

            xml.Save(nombreXML);

            return nombreXML;
        }

    }
}
