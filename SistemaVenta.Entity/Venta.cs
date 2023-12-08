using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity
{
    public partial class Venta
    {
        public Venta()
        {
            DetalleVenta = new HashSet<DetalleVenta>();
        }

        public int IdVenta { get; set; }
        public string? NumeroVenta { get; set; }
        public int? IdTipoDocumentoVenta { get; set; }
        public int? IdUsuario { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ImpuestoTotal { get; set; }
        public decimal? Total { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? CveMetodoPago { get; set; }
        
        public int? IdCliente { get; set; }
        public int? CveFormaPago { get; set; }
        public string? CveUsoCFDI { get; set; }

        public virtual TipoDocumentoVenta? IdTipoDocumentoVentaNavigation { get; set; }
        public virtual Usuario? IdUsuarioNavigation { get; set; }
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public virtual FormaPago? FormaPago { get; set; }
        public virtual UsoCFDI? UsoCFDI { get; set; }
    }
}
