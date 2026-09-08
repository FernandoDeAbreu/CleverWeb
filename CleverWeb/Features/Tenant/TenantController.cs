using CleverWeb.Data;
using CleverWeb.Features.Tenant.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleverWeb.Features.Tenant
{
    [Authorize]
    public class TenantController : Controller
    {
        private readonly CleverDbContext _db;

        public TenantController(CleverDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tenants = _db.Tenant.OrderBy(x => x.Nome).ToList();
            return View(tenants);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TenantViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TenantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var slug = model.Slug.Trim();
            var nome = model.Nome.Trim();

            if (_db.Tenant.Any(x => x.Slug == slug))
            {
                ModelState.AddModelError(nameof(model.Slug), "Este identificador já existe.");
                return View(model);
            }

            var tenant = new Models.Tenant
            {
                Nome = nome,
                Slug = slug,
                Ativo = model.Ativo,
                DataCriacao = DateTime.UtcNow
            };

            _db.Tenant.Add(tenant);
            _db.SaveChanges();

            TempData["Success"] = "Igreja cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var tenant = _db.Tenant.FirstOrDefault(x => x.Id == id);
            if (tenant == null)
                return NotFound();

            return View(new TenantViewModel
            {
                Nome = tenant.Nome,
                Slug = tenant.Slug,
                Ativo = tenant.Ativo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TenantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tenant = _db.Tenant.FirstOrDefault(x => x.Id == id);
            if (tenant == null)
                return NotFound();

            var slug = model.Slug.Trim();
            if (_db.Tenant.Any(x => x.Slug == slug && x.Id != id))
            {
                ModelState.AddModelError(nameof(model.Slug), "Este identificador já existe para outra igreja.");
                return View(model);
            }

            tenant.Nome = model.Nome.Trim();
            tenant.Slug = slug;
            tenant.Ativo = model.Ativo;
            _db.SaveChanges();

            TempData["Success"] = "Igreja atualizada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var tenant = _db.Tenant.FirstOrDefault(x => x.Id == id);
            if (tenant == null)
                return NotFound();

            tenant.Ativo = !tenant.Ativo;
            _db.SaveChanges();

            TempData["Success"] = tenant.Ativo ? "Igreja ativada com sucesso." : "Igreja desativada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var tenant = _db.Tenant.FirstOrDefault(x => x.Id == id);
            if (tenant == null)
                return NotFound();

            var hasRelation = _db.Usuario.Any(x => x.TenantId == id)
                || _db.Membro.Any(x => x.TenantId == id)
                || _db.Contribuicao.Any(x => x.TenantId == id)
                || _db.Despesa.Any(x => x.TenantId == id)
                || _db.Fornecedor.Any(x => x.TenantId == id)
                || _db.Caixa.Any(x => x.TenantId == id);

            if (hasRelation)
            {
                TempData["Success"] = "Não foi possível excluir a igreja porque ela possui registros vinculados.";
                return RedirectToAction(nameof(Index));
            }

            _db.Tenant.Remove(tenant);
            _db.SaveChanges();

            TempData["Success"] = "Igreja removida com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
