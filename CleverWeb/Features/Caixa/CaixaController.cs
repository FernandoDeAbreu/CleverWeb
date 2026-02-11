using CleverWeb.Data.Shared;
using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleverWeb.Features.Caixa
{
    [Authorize]
    public class CaixaController : Controller
    {
        private readonly CaixaService _caixaService;

        public CaixaController(CaixaService caixaService)
        {
            _caixaService = caixaService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _caixaService.HistoricoCaixa();
            return View(vm);
        }
        public async Task<IActionResult> Fechamento(FiltroMovimentoCaixaViewModel filtro)
        {
            if (!filtro.TipoContribuicao.HasValue)
            {
                filtro = new FiltroMovimentoCaixaViewModel
                {
                    TipoContribuicao = Enum
               .GetValues(typeof(Enums.TipoContribuicao))
               .Cast<Enums.TipoContribuicao>()
               .First()
                };
            }

            var vm =  _caixaService.ObterRelatorio(filtro);
            return View(vm);
        }

        public async Task<IActionResult> Movimento(int Id)
        {
            var filtro = new FiltroMovimentoCaixaViewModel
            {
                CaixaId = Id,
                DataFim = null,
                DataInicio = null,
            }            ;

            var vm =  _caixaService.ObterRelatorio(filtro);
            return View(vm);
        }

        public IActionResult ExportarPdf(int Id)
        {
            var filtro = new FiltroMovimentoCaixaViewModel
            {
                CaixaId = Id,
                TipoContribuicao = Enums.TipoContribuicao.Dízimo,
                DataFim = null,
                DataInicio = null,
            };

            var vm = _caixaService.ObterRelatorio(filtro);
            var pdf = _caixaService.ExportarPdf(vm);
            return File(pdf, "application/pdf", "Contribuicoes.pdf");
        }

        public IActionResult FecharCaixa(FiltroMovimentoCaixaViewModel filtro)
        {
            var vm = _caixaService.ObterRelatorio(filtro);
            var pdf = _caixaService.FecharCaixa(vm);
            return RedirectToAction(nameof(Index));
        }
    }
}