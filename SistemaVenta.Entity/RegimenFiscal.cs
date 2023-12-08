using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Entity
{
    public class RegimenFiscal
    {

        public RegimenFiscal() {
            Clientes = new HashSet<Cliente>();
        }

        public int cveRegimen { get; set; }
        public string descripcion { get; set; }

        public virtual ICollection<Cliente> Clientes { get; set; }

    }
}
