namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMDetallesFacturaVenta
    {
        public int IdVenta { get; set; }
        public string? NumeroVenta { get; set; }
        public int? IdTipoDocumentoVenta { get; set; }
        public string? TipoDocumentoVenta { get; set; }
        public int? IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public string? DocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? SubTotal { get; set; }
        public string? ImpuestoTotal { get; set; }
        public string? Total { get; set; }
        public string? FechaRegistro { get; set; }

        public int? IdCliente { get; set; }
        public int? CveRegimen { get; set; }
        public string? Regimen { get; set; }
        public string? RFC { get; set; }
        public string? CodigoPostal { get; set; }
        public int? CveFormaPago { get; set; }
        public string? FormaPago { get; set; }
        public string? CveMetodoPago { get; set; }
        public string? MetodoPago { get; set; }
        public string? CveUsoCFDI { get; set; }
        public string? UsoCFDI { get; set; }

        public virtual ICollection<VMDetalleVenta> DetalleVenta { get; set; }
    }
}
