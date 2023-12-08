using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class ClienteService : IClienteService
    {
        private readonly IGenericRepository<Cliente> _repository;
        public ClienteService(IGenericRepository<Cliente> repository) {
            this._repository = repository;
        }

        public async Task<Cliente> Crear(Cliente cliente)
        {
            // REVISAMOS SI YA EXISTE EL CLIENTE CON RFC Y REGIMEN
            Cliente clienteExistente = await this._repository.Obtener(c => c.rfc == cliente.rfc && c.Regimen == cliente.Regimen);
            if (clienteExistente != null) throw new TaskCanceledException("Ya está registrado ese RFC con ese Regimen");
            Cliente clienteCreado = await this._repository.Crear(cliente);
            if (clienteCreado.idCliente == 0) throw new TaskCanceledException("No se pudo registrar al cliente");
            return clienteCreado;
        }

        public async Task<Cliente> Editar(Cliente cliente)
        {
            // Revisamos que el cliente exista
            Cliente clienteExistente = await this._repository.Obtener(c => c.idCliente == cliente.idCliente );
            if (clienteExistente == null) throw new TaskCanceledException("El cliente a editar no existe");
            await this._repository.Editar(cliente);
            return cliente;
        }

        public async Task<bool> Eliminar(int idCliente)
        {
            // Revisamos que el cliente exista
            Cliente clienteExistente = await this._repository.Obtener(c => c.idCliente == idCliente);
            if (clienteExistente == null) throw new TaskCanceledException("El cliente a eliminnar no existe");
            await this._repository.Eliminar(clienteExistente);
            return true;
        }

        public async Task<List<Cliente>> ListarClientes()
        {
            IQueryable<Cliente> query =  await this._repository.Consultar();
            return query.ToList();
        }

        public async Task<Cliente> ObtenerPorId(int idCliente)
        {
            return await this._repository.Obtener(c => c.idCliente == idCliente);
        }
    }
}
