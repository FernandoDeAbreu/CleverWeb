using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Fornecedor.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Features.Fornecedor
{
    [Authorize]
    public class FornecedorController : Controller
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;

        public FornecedorController(CleverDbContext db, IMapper mapper, ITenantAccessor tenantAccessor)
        {
            _db = db;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var fornecedores = await _db.Fornecedor
                .Where(fornecedor => fornecedor.TenantId == tenantId)
                .AsNoTracking()
                .OrderBy(fornecedor => fornecedor.Descricao)
                .ToListAsync();

            return View(_mapper.Map<List<FornecedorViewModel>>(fornecedores));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new FornecedorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FornecedorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var fornecedor = new Models.Fornecedor
            {
                Descricao = model.Descricao.Trim(),
                TenantId = _tenantAccessor.CurrentTenantId ?? 0
            };

            _db.Fornecedor.Add(fornecedor);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Fornecedor cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var fornecedor = await _db.Fornecedor
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

            if (fornecedor == null)
                return NotFound();

            return View(_mapper.Map<FornecedorViewModel>(fornecedor));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FornecedorViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var fornecedor = await _db.Fornecedor
                .FirstOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

            if (fornecedor == null)
                return NotFound();

            fornecedor.Descricao = model.Descricao.Trim();
            await _db.SaveChangesAsync();

            TempData["Success"] = "Fornecedor atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var fornecedor = await _db.Fornecedor
                .FirstOrDefaultAsync(item => item.Id == id && item.TenantId == tenantId);

            if (fornecedor == null)
                return NotFound();

            if (await _db.Despesa.AnyAsync(despesa => despesa.FornecedorId == id && despesa.TenantId == tenantId))
            {
                TempData["Error"] = "Não foi possível excluir o fornecedor porque existem despesas vinculadas a ele.";
                return RedirectToAction(nameof(Index));
            }

            _db.Fornecedor.Remove(fornecedor);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Fornecedor removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
