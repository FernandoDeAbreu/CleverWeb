using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Contribuicao.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CleverWeb.Features.Caixa
{
    public class CaixaController : Controller
    {
        private readonly CaixaService _caixaService;
        public CaixaController(CaixaService caixaService)
        {
            _caixaService = caixaService;
        }
       
        public IActionResult Index(FiltroMovimentoCaixaViewModel filtro)
        {
            var vm = _caixaService.ObterRelatorio(filtro);
            return View(vm);
        }
        public IActionResult ExportarPdf(FiltroMovimentoCaixaViewModel filtro)
        {
            var vm = _caixaService.ObterRelatorio(filtro);
            var pdf = _caixaService.ExportarPdf(vm);
            return File(pdf, "application/pdf", "Contribuicoes.pdf");
        }
    }
}
