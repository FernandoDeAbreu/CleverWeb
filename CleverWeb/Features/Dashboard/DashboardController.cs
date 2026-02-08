using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Home
{
    //  [Authorize]

    public class DashboardController : Controller
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;

        public DashboardController(CleverDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var TotalCaixaOfetas = await _db.Contribuicao
                .Where(c => c.TipoContribuicao == TipoContribuicao.Oferta && 
                            c.MotivoExclusao == null)
                .SumAsync(x => (double)x.Valor);

            var saldoCaixaDizimo = await _db.Caixa
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync();

            var dashboard = new DashboardViewModel
            {
                SaldoCaixaDizimo = saldoCaixaDizimo.Saldo,
                SaldoCaixaOfertas = TotalCaixaOfetas,
            };

            return View(dashboard);
        }
    }
}