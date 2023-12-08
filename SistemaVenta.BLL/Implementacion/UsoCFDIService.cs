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
    public class UsoCFDIService: IUsoCFDIService
    {
        private readonly IGenericRepository<UsoCFDI> _repository;

        public UsoCFDIService(IGenericRepository<UsoCFDI> repository)
        {
            this._repository = repository;
        }
        
        public async Task<List<UsoCFDI>> Lista()
        {
            IQueryable<UsoCFDI> query = await this._repository.Consultar();

            return query.ToList();
        }
    }
}
