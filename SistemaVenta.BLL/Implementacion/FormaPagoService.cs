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
    public class FormaPagoService: IFormaPagoService
    {
        private readonly IGenericRepository<FormaPago> _repository;

        public FormaPagoService(IGenericRepository<FormaPago> repository)
        {
            this._repository = repository;
        }

        public async Task<List<FormaPago>> Lista()
        {
            IQueryable<FormaPago> query = await this._repository.Consultar();

            return query.ToList();
        }
    }
}
