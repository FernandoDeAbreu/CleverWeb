using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Home
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly CleverDbContext _db;
        private readonly CaixaService _caixaService;
        private readonly IMapper _mapper;

        public DashboardController(CleverDbContext db, IMapper mapper, CaixaService caixaService)
        {
            _db = db;
            _mapper = mapper;
            _caixaService = caixaService;
        }

        public async Task<IActionResult> Index()
        {
            var dizimo = await _db.Caixa
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync(c => c.TipoContribuicao == TipoContribuicao.Dízimo) ?? new Models.Caixa();

            var oferta = await _db.Caixa
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync(c => c.TipoContribuicao == TipoContribuicao.Oferta) ?? new Models.Caixa(); 

            var missao = await _db.Caixa
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync(c => c.TipoContribuicao == TipoContribuicao.Missão) ?? new Models.Caixa(); 

            var filto = new FiltroMovimentoCaixaViewModel {
                DataInicio = DateTime.Now.AddDays(-7),
                DataFim = DateTime.Now.AddDays(1),
            };

            var ultimasAtividades = _caixaService.ObterRelatorio(filto);

            var dashboard = new DashboardViewModel
            {
                SaldoCaixaDizimo = dizimo.SaldoAtual,
                SaldoCaixaOfertas = oferta.SaldoAtual,
                SaldoCaixaMissao = missao.SaldoAtual,
                Lista = ultimasAtividades.Lista.Take(10)
            };

            return View(dashboard);
        }
    }
}