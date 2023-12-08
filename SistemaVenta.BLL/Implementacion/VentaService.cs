using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private readonly IVentaRepository _repositorioVenta;
        private readonly IFormaPagoService _formaPagoServicio;
        private readonly IUsoCFDIService _usoCFDIService;
        private readonly IClienteService _clienteService;
        private readonly IRegimenFiscalService _regimenService;

        public VentaService(IGenericRepository<Producto> repositorioProducto,
            IVentaRepository repositorioVenta,
            IFormaPagoService formaPagoService,
            IUsoCFDIService usoCFDIService,
            IClienteService clienteService,
            IRegimenFiscalService regimenFiscalService
            )
        {
            _repositorioProducto = repositorioProducto;
            _repositorioVenta = repositorioVenta;
            _formaPagoServicio = formaPagoService;
            _usoCFDIService = usoCFDIService;
            _clienteService = clienteService;
            _regimenService = regimenFiscalService;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar( p => 
                p.EsActivo == true &&
                p.Stock > 0 &&
                string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)
                );

                return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        public async Task<DetallesFacturaVenta> GetDetallesFactura(DetallesFacturaVenta venta)
        {

            if (venta.IdCliente == null) return venta;
            if (venta.CveFormaPago == null) return venta;
            if (venta.CveUsoCFDI == null) return venta;
            if (venta.CveMetodoPago == null) return venta;

            Cliente? cliente = await this._clienteService.ObtenerPorId((int)venta.IdCliente);

            if (cliente == null) return venta;

            RegimenFiscal? regimen = (await this._regimenService.Lista()).FirstOrDefault(x => x.cveRegimen == cliente.cveRegimen, null);

            if (regimen == null) return venta;

            FormaPago? formaPago = (await this._formaPagoServicio.Lista()).FirstOrDefault(x => x.cveFormaPago == venta.CveFormaPago, null);
            if (formaPago == null) return venta;

            UsoCFDI? usoCFDI = (await this._usoCFDIService.Lista()).FirstOrDefault(x => x.claveUsoCFDI == venta.CveUsoCFDI, null);
            if (usoCFDI == null) return venta;

            venta.NombreCliente = cliente.nombre;
            venta.RFC = cliente.rfc;
            venta.CodigoPostal = cliente.codigo_postal;
            venta.CveRegimen = regimen.cveRegimen;
            venta.Regimen = venta.CveRegimen + "-" + regimen.descripcion;
            venta.FormaPago = formaPago.descripcion;
            venta.UsoCFDI = usoCFDI.descripcion;
            venta.MetodoPago = venta.CveMetodoPago == "PPD" ? "PPD" : "PUE";
            return venta;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositorioVenta.Registrar(entidad);
            }
            catch {
                throw;
            }
        }


        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;


            if (fechaInicio != "" && fechaFin != "")
            {

                DateTime fech_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                DateTime fech_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                return query.Where(v =>
                    v.FechaRegistro.Value.Date >= fech_inicio.Date &&
                    v.FechaRegistro.Value.Date <= fech_fin.Date
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
            else {
                return query.Where(v => v.NumeroVenta == numeroVenta
                   )
                       .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                       .Include(u => u.IdUsuarioNavigation)
                       .Include(dv => dv.DetalleVenta)
                       .ToList();
            }

        }

        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.NumeroVenta == numeroVenta);

           return query
                       .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                       .Include(u => u.IdUsuarioNavigation)
                       .Include(dv => dv.DetalleVenta)
                       .First();
        }

    
        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime fech_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
            DateTime fech_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

            List<DetalleVenta> lista = await _repositorioVenta.Reporte(fech_inicio, fech_fin);

            return lista;
        }

    }
}
