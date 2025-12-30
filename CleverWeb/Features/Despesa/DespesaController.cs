using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Despesa.Services;
using CleverWeb.Features.Despesa.ViewModels;
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

        public DespesaController(DespesaService DespesaService, CleverDbContext db, IMapper mapper)
        {
            _despesaService = DespesaService;
            _db = db;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var Despesa = await _db.Despesa.Where(c => c.MotivoExclusao == null)
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
                return View(model);

            await _despesaService.Create(model);

            return RedirectToAction(nameof(Index));
        }
    }
}