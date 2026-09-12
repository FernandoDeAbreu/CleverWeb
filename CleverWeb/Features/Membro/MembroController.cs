using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Membro.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Features.Membro
{

    [Authorize]
    public class MembroController : Controller
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;

        public MembroController(CleverDbContext db, IMapper mapper, ITenantAccessor tenantAccessor)
        {
            _db = db;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var membros = await _db.Membro
                .Where(m => m.TenantId == tenantId)
                .AsNoTracking()
                .OrderBy(m => m.Nome)
                .ToListAsync();

            var vm = _mapper.Map<List<MembroViewModel>>(membros);
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var membro = await _db.Membro
                .Where(m => m.TenantId == tenantId)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (membro == null)
                return NotFound();

            return View(_mapper.Map<MembroViewModel>(membro));
        }

        public IActionResult Create()
        {
            var model = new MembroViewModel
            {
                TenantId = _tenantAccessor.CurrentTenantId ?? 0
            };

            CarregarOpcoesTenant(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembroViewModel model)
        {
            var tenantId = ObterTenantParaCadastro(model);
            if (tenantId <= 0)
                ModelState.AddModelError(nameof(model.TenantId), "Selecione uma igreja válida para o membro.");
            else if (!_db.Tenant.Any(tenant => tenant.Id == tenantId && tenant.Ativo))
                ModelState.AddModelError(nameof(model.TenantId), "Essa igreja não está ativa ou não existe.");

            if (!ModelState.IsValid)
            {
                CarregarOpcoesTenant(model);
                return View(model);
            }

            var entidade = _mapper.Map<Models.Membro>(model);
            entidade.DataCadastro = DateTime.UtcNow;
            entidade.TenantId = tenantId;

            _db.Membro.Add(entidade);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Membro cadastrado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        private int ObterTenantParaCadastro(MembroViewModel model)
        {
            if (User.HasClaim("is_global_admin", "true"))
                return model.TenantId;

            model.TenantId = _tenantAccessor.CurrentTenantId ?? 0;
            return model.TenantId;
        }

        private void CarregarOpcoesTenant(MembroViewModel model)
        {
            if (!User.HasClaim("is_global_admin", "true"))
                return;

            model.TenantOptions = _db.Tenant
                .Where(tenant => tenant.Ativo)
                .OrderBy(tenant => tenant.Nome)
                .Select(tenant => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = tenant.Id.ToString(),
                    Text = tenant.Nome,
                    Selected = tenant.Id == model.TenantId
                })
                .ToList();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var membro = await _db.Membro.FindAsync(id);

            if (membro == null)
                return NotFound();

            return View(_mapper.Map<MembroViewModel>(membro));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MembroViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var entidade = await _db.Membro.FindAsync(id);

            if (entidade == null)
                return NotFound();

            _mapper.Map(model, entidade);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Membro atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var membro = await _db.Membro
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (membro == null)
                return NotFound();

            return View(_mapper.Map<MembroViewModel>(membro));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membro = await _db.Membro.FindAsync(id);

            if (membro == null)
                return NotFound();

            _db.Membro.Remove(membro);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Membro removido com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}