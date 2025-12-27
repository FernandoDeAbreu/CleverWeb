using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Data.Reports;
using CleverWeb.Features.Contribuicao.Services;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Membro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Globalization;

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
            var Contribuicao = await _db.Contribuicao
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

        public IActionResult Edit(int id)
        {
            var contribuicao = _db.Contribuicao.Include(c => c.Membro).FirstOrDefault(m => m.Id == id);

            if (contribuicao == null)
                return NotFound();

            ViewBag.Membro = contribuicao.Membro.Nome;
            var vm = _mapper.Map<ContribuicaoViewModel>(contribuicao);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContribuicaoViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var entidade = await _db.Contribuicao.FindAsync(id);
            if (entidade == null)
                return NotFound();

            _mapper.Map(model, entidade);

            _db.Contribuicao.Update(entidade);
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

        public async Task<IActionResult> Imprimir(int id)
        {
            var contribuicao = await _db.Contribuicao
                 .Include(c => c.Membro)
                 .FirstAsync(c => c.Id == id);

            // Gera PDF
            var document = new ReciboContribuicaoReport(contribuicao);
            var pdf = document.GeneratePdf();
            RedirectToAction(nameof(MembroList));

            return File(pdf, "application/pdf", $"{contribuicao.TipoContribuicao}-{contribuicao.Id}.pdf");
        }

        public IActionResult Relatorio(FiltroContribuicaoViewModel filtro)
        {
            var lista =  _contribuicaoService.Relatorio(filtro);
            return View(lista);
        }

        public async Task<IActionResult> ExportarPdfAsync(FiltroContribuicaoViewModel filtro)
        {
            var pdfBytes = await _contribuicaoService.ExportarPdf(filtro);
            return File(pdfBytes, "application/pdf", "Contribuicoes.pdf");
        }
    }
}