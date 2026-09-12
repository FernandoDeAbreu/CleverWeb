using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Contribuicao.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Membro.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using CleverWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Contribuicao
{
    [Authorize]
    public class ContribuicaoController : Controller
    {
        private readonly CleverDbContext _db;
        private readonly ContribuicaoService _contribuicaoService;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;
        private readonly CaixaService _caixaService;

        public ContribuicaoController(ContribuicaoService contribuicaoService, CleverDbContext db, IMapper mapper, ITenantAccessor tenantAccessor, CaixaService caixaService)
        {
            _contribuicaoService = contribuicaoService;
            _db = db;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
            _caixaService = caixaService;
        }
   
        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var Contribuicao = await _db.Contribuicao.Where(c => c.MotivoExclusao == null && c.TenantId == tenantId)
                 .Include(c => c.Membro)
                .AsNoTracking()
                .OrderByDescending(m => m.DataLancamanto).Take(20)
                .ToListAsync();

            var vm = _mapper.Map<List<ContribuicaoViewModel>>(Contribuicao);
            return View(vm);
        }

        public async Task<IActionResult> MembroList(string? nome)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var consulta = _db.Membro
                .Where(m => m.TenantId == tenantId);

            if (!string.IsNullOrWhiteSpace(nome))
                consulta = consulta.Where(m => m.Nome.Contains(nome));

            var Membro = await consulta
                .AsNoTracking()
                .OrderBy(m => m.Nome)
                .ToListAsync();

            ViewBag.NomePesquisa = nome;
            var vm = _mapper.Map<List<MembroViewModel>>(Membro);
            return View(vm);
        }

        public IActionResult Create(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var membro = _db.Membro.FirstOrDefault(m => m.Id == id && m.TenantId == tenantId);

            ViewBag.Membro = membro?.Nome;
            ViewBag.MembroId = id;
            return View(new ContribuicaoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int membroId, ContribuicaoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var contribuicao = new Models.Contribuicao
            {
                TenantId = _tenantAccessor.CurrentTenantId ?? 0,
                MembroId = membroId,
                Valor = model.Valor,
                DataLancamanto = DateTime.UtcNow,
                TipoContribuicao = model.TipoContribuicao,
                FormaPagto = model.FormaPagto,
                DataPagamento = model.DataPagamento
            };

            _db.Contribuicao.Add(contribuicao);
            await _db.SaveChangesAsync();
            await _caixaService.AtualizarSaldoAtual(contribuicao.TipoContribuicao, contribuicao.Valor);

            return RedirectToAction("Comprovante", new { id = contribuicao.Id });
        }

        public IActionResult Details(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var contribuicao = _db.Contribuicao.Include(c => c.Membro).FirstOrDefault(m => m.Id == id && m.TenantId == tenantId);
            ViewBag.MembroId = contribuicao?.MembroId;
            ViewBag.Membro = contribuicao?.Membro.Nome;
            var vm = _mapper.Map<ContribuicaoViewModel>(contribuicao);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Estornar(int id, string? motivoExclusao)
        {
            if (string.IsNullOrWhiteSpace(motivoExclusao) || motivoExclusao.Trim().Length < 15)
            {
                TempData["Error"] = "O motivo do estorno deve ser informado e conter no mínimo 15 caracteres.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _contribuicaoService.Estornar(id, motivoExclusao);
                TempData["Success"] = "Contribuição estornada com sucesso.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Comprovante(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var contribuicao = await _db.Contribuicao
                .Include(c => c.Membro)
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

            if (contribuicao == null)
                return NotFound();

            return View(contribuicao);
        }

        public async Task<IActionResult> ImprimirComprovante(int id)
        {
            var document = await _contribuicaoService.ImprimirComprovante(id);


            return File(document, "application/pdf", $"Recibo-{id}.pdf");

        }

        public IActionResult Relatorio(FiltroContribuicaoViewModel filtro)
        {
            var vm = _contribuicaoService.ObterRelatorio(filtro);
            return View(vm);
        }

        public IActionResult ExportarPdf(FiltroContribuicaoViewModel filtro)
        {
            var vm = _contribuicaoService.ObterRelatorio(filtro);
            var pdf =  _contribuicaoService.ExportarPdf(vm);
            return File(pdf, "application/pdf", "Contribuicoes.pdf");
        }

    }
}