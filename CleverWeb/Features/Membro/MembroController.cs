using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Membro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Features.Membro;

[Authorize]
public class MembroController : Controller
{
    private readonly CleverDbContext _db;
    private readonly IMapper _mapper;

    public MembroController(CleverDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var membros = await _db.Membro
            .AsNoTracking()
            .OrderBy(m => m.Nome)
            .ToListAsync();

        var vm = _mapper.Map<List<MembroViewModel>>(membros);
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var membro = await _db.Membro
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membro == null)
            return NotFound();

        return View(_mapper.Map<MembroViewModel>(membro));
    }

    public IActionResult Create()
    {
        return View(new MembroViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MembroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var entidade = _mapper.Map<Models.Membro>(model);
        entidade.DataCadastro = DateTime.UtcNow;

        _db.Membro.Add(entidade);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Membro cadastrado com sucesso!";

        return RedirectToAction(nameof(Index));
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