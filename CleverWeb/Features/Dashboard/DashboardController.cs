using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
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
            var tenantId = User.FindFirst("tenant_id")?.Value;
            var tenantFilter = string.IsNullOrWhiteSpace(tenantId) ? 0 : int.Parse(tenantId);

            var dizimo = await _db.Caixa
                  .Where(c => c.TenantId == tenantFilter)
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync(c => c.TipoContribuicao == TipoContribuicao.Dízimo) ?? new Models.Caixa();

            var oferta = await _db.Caixa
                  .Where(c => c.TenantId == tenantFilter)
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync(c => c.TipoContribuicao == TipoContribuicao.Oferta) ?? new Models.Caixa(); 

            var missao = await _db.Caixa
                  .Where(c => c.TenantId == tenantFilter)
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefaultAsync(c => c.TipoContribuicao == TipoContribuicao.Missão) ?? new Models.Caixa(); 

            var filto = new FiltroMovimentoCaixaViewModel {
                DataInicio = DateTime.Now.AddDays(-7),
                DataFim = DateTime.Now.AddDays(1),
            };

            var ultimasAtividades = _caixaService.ObterRelatorio(filto);

            var hoje = DateTime.Today;
            var inicioSeteDias = hoje.AddDays(-6);
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var periodos = Enumerable.Range(0, 7)
                .Select(indice => inicioSeteDias.AddDays(indice))
                .ToList();

            var membrosQuery = _db.Membro.Where(m => m.TenantId == tenantFilter);
            var totalMembros = await membrosQuery.CountAsync();
            var novosMembrosNoMes = await membrosQuery.CountAsync(m => m.DataCadastro >= inicioMes);

            var contribuicoesSeteDias = await _db.Contribuicao
                .Where(c => c.TenantId == tenantFilter && c.MotivoExclusao == null && c.DataPagamento >= inicioSeteDias)
                .Select(c => new { c.DataPagamento, c.Valor })
                .ToListAsync();
            var despesasSeteDias = await _db.Despesa
                .Where(d => d.TenantId == tenantFilter && d.MotivoExclusao == null && d.DataPagamento >= inicioSeteDias)
                .Select(d => new { d.DataPagamento, d.Valor })
                .ToListAsync();

            var entradasPorPeriodo = periodos.Select(periodo => contribuicoesSeteDias
                .Where(item => item.DataPagamento.Date == periodo.Date)
                .Sum(item => item.Valor)).ToList();
            var saidasPorPeriodo = periodos.Select(periodo => despesasSeteDias
                .Where(item => item.DataPagamento.Date == periodo.Date)
                .Sum(item => item.Valor)).ToList();
            var membrosPorPeriodo = periodos.Select(periodo => membrosQuery.Count(m => m.DataCadastro.Date == periodo.Date)).ToList();

            var dashboard = new DashboardViewModel
            {
                SaldoCaixaDizimo = dizimo.SaldoAtual,
                SaldoCaixaOfertas = oferta.SaldoAtual,
                SaldoCaixaMissao = 0,
                TotalMembros = totalMembros,
                NovosMembrosNoMes = novosMembrosNoMes,
                EntradasUltimosSeteDias = contribuicoesSeteDias.Sum(item => item.Valor),
                SaidasUltimosSeteDias = despesasSeteDias.Sum(item => item.Valor),
                Periodos = periodos.Select(periodo => periodo.ToString("dd/MM", CultureInfo.InvariantCulture)).ToList(),
                EntradasPorPeriodo = entradasPorPeriodo,
                SaidasPorPeriodo = saidasPorPeriodo,
                MembrosPorPeriodo = membrosPorPeriodo,
                Lista = ultimasAtividades.Lista.Take(10)
            };

            return View(dashboard);
        }
    }
}