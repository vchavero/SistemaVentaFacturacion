using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMCliente
    {
        public int idCliente { get; set; }
        public string nombre { get; set; }
        public string rfc { get; set; }
        public string codigo_postal { get; set; }
        public int cveRegimen { get; set; }
        public virtual RegimenFiscal Regimen { get; set; }

    }
}
