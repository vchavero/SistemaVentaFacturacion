using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IClienteService
    {
        public Task<Cliente> Crear(Cliente cliente);
        public Task<Cliente> Editar(Cliente cliente);
        public Task<bool> Eliminar(int idCliente);
        public Task<Cliente> ObtenerPorId(int idCliente);
        public Task<List<Cliente>> ListarClientes();
    }
}
