using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Contribuicao.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Membro.ViewModels;
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

        public ContribuicaoController(ContribuicaoService contribuicaoService, CleverDbContext db, IMapper mapper)
        {
            _contribuicaoService = contribuicaoService;
            _db = db;
            _mapper = mapper;
        }
   
        public async Task<IActionResult> Index()
        {
            var Contribuicao = await _db.Contribuicao.Where(c => c.MotivoExclusao == null)
                 .Include(c => c.Membro)
                .AsNoTracking()
                .OrderByDescending(m => m.DataLancamanto).Take(20)
                .ToListAsync();

            var vm = _mapper.Map<List<ContribuicaoViewModel>>(Contribuicao);
            return View(vm);
        }

        public async Task<IActionResult> MembroList()
        {
            var Membro = await _db.Membro
                .AsNoTracking()
                .OrderBy(m => m.Nome)
                .ToListAsync();

            var vm = _mapper.Map<List<MembroViewModel>>(Membro);
            return View(vm);
        }

        public IActionResult Create(int id)
        {
            var membro = _db.Membro.FirstOrDefault(m => m.Id == id);

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
                MembroId = membroId,
                Valor = model.Valor,
                DataLancamanto = DateTime.UtcNow,
                TipoContribuicao = model.TipoContribuicao,
                FormaPagto = model.FormaPagto,
                DataPagamento = model.DataPagamento
            };

            _db.Contribuicao.Add(contribuicao);
            await _db.SaveChangesAsync();

            return RedirectToAction("Comprovante", new { id = contribuicao.Id });
        }

        public IActionResult Details(int id)
        {
            var contribuicao = _db.Contribuicao.Include(c => c.Membro).FirstOrDefault(m => m.Id == id);
            ViewBag.MembroId = contribuicao?.MembroId;
            ViewBag.Membro = contribuicao?.Membro.Nome;
            var vm = _mapper.Map<ContribuicaoViewModel>(contribuicao);
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contribuicao = await _db.Contribuicao.FindAsync(id); 

            if (contribuicao == null)
                return NotFound();

            ViewBag.MembroId = contribuicao.MembroId;

            var vm = _mapper.Map<ContribuicaoViewModel>(contribuicao);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContribuicaoViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var entidade = await _db.Contribuicao.FindAsync(id);
           
            if (entidade == null)
                return NotFound();

            model.DataExclusao = DateTime.Now;

            _mapper.Map(model, entidade);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Contribuicao atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Comprovante(int id)
        {
            var contribuicao = await _db.Contribuicao
                .Include(c => c.Membro)
                .FirstOrDefaultAsync(c => c.Id == id);

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