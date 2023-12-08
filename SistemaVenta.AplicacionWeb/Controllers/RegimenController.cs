using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class RegimenController: Controller
    {

        private readonly IRegimenFiscalService _registrimenFiscalService;
        private readonly IMapper _mapper;

        public RegimenController(IRegimenFiscalService registrimenFiscalService, IMapper mapper)
        {
            _registrimenFiscalService = registrimenFiscalService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Lista() {
            List<VMRegimenFiscal> regimenes = this._mapper.Map<List<VMRegimenFiscal>>(await this._registrimenFiscalService.Lista());
            return StatusCode(StatusCodes.Status200OK, regimenes);
        }

    }
}
