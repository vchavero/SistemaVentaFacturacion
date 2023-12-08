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
    public class RegimenFiscalService : IRegimenFiscalService
    {

        private readonly IGenericRepository<RegimenFiscal> _repository;

        public RegimenFiscalService(IGenericRepository<RegimenFiscal> repository)
        {
            this._repository = repository;
        }

        public async Task<List<RegimenFiscal>> Lista()
        {
            IQueryable<RegimenFiscal> query = await this._repository.Consultar();

            return query.ToList();
        }
    }
}
