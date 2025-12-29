using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Data.Reports;
using CleverWeb.Features.Contribuicao.ViewModels;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace CleverWeb.Features.Contribuicao.Services
{
    public class ContribuicaoService
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;

        public ContribuicaoService(CleverDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public RelatorioContribuicaoViewModel ObterRelatorio(FiltroContribuicaoViewModel filtro)
        {
                 var query = _db.Contribuicao
               .Include(c => c.Membro)
               .AsNoTracking()
               .OrderByDescending(c => c.DataPagamento)
               .AsQueryable();

                   if (filtro.TipoContribuicao.HasValue)
                       query = query.Where(x => x.TipoContribuicao == filtro.TipoContribuicao);

                   if (filtro.DataInicio.HasValue)
                       query = query.Where(x => x.DataPagamento >= filtro.DataInicio);

                   if (filtro.DataFim.HasValue)
                       query = query.Where(x => x.DataPagamento <= filtro.DataFim);

                   var lista = _mapper.Map<List<ContribuicaoViewModel>>(query.ToList());

                   return new RelatorioContribuicaoViewModel
                   {
                       Filtro = filtro,
                       Lista = lista
                   };
        }

        public async Task<byte[]> ImprimirComprovante(int id)
        {
            var contribuicao = await _db.Contribuicao
                .Include(c => c.Membro)
                .FirstAsync(c => c.Id == id);

            var document = new ReciboContribuicaoReport(contribuicao);
            return document.GeneratePdf();
        }

        public byte[] ExportarPdf(RelatorioContribuicaoViewModel relatorioContribuicao)
        {
            var document = new RelatorioTemploCentral(relatorioContribuicao);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}