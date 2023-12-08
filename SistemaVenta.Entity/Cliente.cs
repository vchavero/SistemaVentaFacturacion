using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Entity
{
    public partial class Cliente
    {
        public Cliente() {
            Ventas = new HashSet<Venta>();
        }

        public int idCliente { get; set; }
        public string nombre { get; set; }
        public string rfc { get; set; }
        public string codigo_postal { get; set; }
        public int? cveRegimen { get; set; }
        public virtual RegimenFiscal Regimen { get; set; }
        public virtual ICollection<Venta> Ventas { get; set; }

    }
}
