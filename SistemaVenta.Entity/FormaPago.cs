using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.Entity
{
    public partial class FormaPago
    {
        public FormaPago() {
            Ventas = new HashSet<Venta>();
        }
        public int cveFormaPago { get; set; }
        public string descripcion { get; set; }
        public virtual ICollection<Venta> Ventas { get; set; }
    }
}
