using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Entity
{
    public partial class UsoCFDI
    {
        public UsoCFDI() {
            Ventas = new HashSet<Venta>();
        }

        public string claveUsoCFDI;
        public string descripcion;
        public virtual ICollection<Venta> Ventas { get; set; }
    }
}
