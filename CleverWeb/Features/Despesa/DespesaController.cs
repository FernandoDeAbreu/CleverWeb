using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Despesa.Services;
using CleverWeb.Features.Despesa.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Features.Despesa
{
    [Authorize]
    public class DespesaController : Controller
    {
        private readonly CleverDbContext _db;
        private readonly DespesaService _despesaService;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;

        public DespesaController(DespesaService DespesaService, CleverDbContext db, IMapper mapper, ITenantAccessor tenantAccessor)
        {
            _despesaService = DespesaService;
            _db = db;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = HttpContext.User.FindFirst("tenant_id")?.Value;
            var tenantFilter = string.IsNullOrWhiteSpace(tenantId) ? 0 : int.Parse(tenantId);

            var Despesa = await _db.Despesa.Where(c => c.MotivoExclusao == null && c.TenantId == tenantFilter)
                .Include(c => c.Fornecedor)
                .AsNoTracking()
                .OrderByDescending(m => m.DataPagamento).Take(20)
                .ToListAsync();



            var vm = _mapper.Map<List<DespesaViewModel>>(Despesa);
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _despesaService.Create();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DespesaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.FornecedorList = await _db.Fornecedor
                    .Where(f => f.TenantId == (_tenantAccessor.CurrentTenantId ?? 0))
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.Descricao
                    })
                    .ToListAsync();

                return View(model);
            }

            await _despesaService.Create(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var despesa = await _db.Despesa
                .Include(d => d.Fornecedor)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);

            if (despesa == null)
                return NotFound();

            return View(_mapper.Map<DespesaViewModel>(despesa));
        }

        [HttpGet]
        public async Task<IActionResult> Estornar(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var despesa = await _db.Despesa
                .Include(d => d.Fornecedor)
                .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);

            if (despesa == null)
                return NotFound();

            return View(_mapper.Map<DespesaViewModel>(despesa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Estornar(int id, DespesaViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _despesaService.Estornar(id, model.MotivoExclusao);
                TempData["Success"] = "Despesa estornada com sucesso.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}