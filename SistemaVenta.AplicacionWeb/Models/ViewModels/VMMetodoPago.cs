namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMMetodoPago
    {
        public string claveMetodoPago { get; set; }
        public string descripcion { get; set; }

        public VMMetodoPago(string clave, string descripcion) { 
            this.claveMetodoPago = clave;
            this.descripcion = descripcion;
        }

    }
}
