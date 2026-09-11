using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Data.Reports;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace CleverWeb.Features.Contribuicao.Services
{
    public class ContribuicaoService
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;

        public ContribuicaoService(CleverDbContext db, IMapper mapper, ITenantAccessor tenantAccessor)
        {
            _db = db;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
        }

        public RelatorioContribuicaoViewModel ObterRelatorio(FiltroContribuicaoViewModel filtro)
        {
                 var tenantId = _db.QueryByTenant(_db.Contribuicao).Select(c => c.TenantId).FirstOrDefault();
                 var query = _db.Contribuicao.Where(c => c.MotivoExclusao == null && c.TenantId == tenantId)
                                            .Include(c => c.Membro)
                                            .AsNoTracking()
                                            .OrderBy(c => c.Membro.Nome)
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
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var contribuicao = await _db.Contribuicao.Where(c => c.MotivoExclusao == null && c.TenantId == tenantId)
                .Include(c => c.Membro)
                .FirstAsync(c => c.Id == id);

            var tenantName = _db.Tenant.FirstOrDefault(t => t.Id == tenantId)?.Nome ?? "Tenant";
            var document = new ReciboContribuicaoReport(contribuicao, tenantName);
            return document.GeneratePdf();
        }

        public byte[] ExportarPdf(RelatorioContribuicaoViewModel relatorioContribuicao)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var tenantName = _db.Tenant.FirstOrDefault(t => t.Id == tenantId)?.Nome ?? "Tenant";
            var document = new RelatorioTemploCentral(relatorioContribuicao, tenantName);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}