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

            var tenant = _db.Tenant.FirstOrDefault(t => t.Id == tenantId);
            var document = new ReciboContribuicaoReport(
                contribuicao,
                tenant?.Nome ?? "Tenant",
                tenant?.Endereco ?? string.Empty,
                tenant?.PastorCongregacional ?? string.Empty);
            return document.GeneratePdf();
        }

        public async Task Estornar(int id, string? motivo)
        {
            var motivoNormalizado = motivo?.Trim();
            if (string.IsNullOrWhiteSpace(motivoNormalizado) || motivoNormalizado.Length < 15)
                throw new InvalidOperationException("O motivo do estorno deve ser informado e conter no mínimo 15 caracteres.");

            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var contribuicao = await _db.Contribuicao
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

            if (contribuicao == null)
                throw new InvalidOperationException("Contribuição não encontrada.");

            if (!string.IsNullOrWhiteSpace(contribuicao.MotivoExclusao))
                throw new InvalidOperationException("Esta contribuição já foi estornada.");

            if (contribuicao.CaixaID > 0)
                throw new InvalidOperationException("Não é possível estornar uma contribuição vinculada a um caixa já fechado. Cancele o fechamento do caixa primeiro.");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            var caixa = contribuicao.CaixaID > 0
                ? await _db.Caixa.FirstOrDefaultAsync(c => c.Id == contribuicao.CaixaID && c.TenantId == tenantId)
                : await _db.Caixa
                    .Where(c => c.TenantId == tenantId
                        && c.TipoContribuicao == contribuicao.TipoContribuicao
                        && !c.Cancelado)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();

            if (caixa != null && !caixa.Cancelado)
                caixa.SaldoAtual -= contribuicao.Valor;

            contribuicao.MotivoExclusao = motivoNormalizado;
            contribuicao.DataExclusao = DateTime.Now;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public byte[] ExportarPdf(RelatorioContribuicaoViewModel relatorioContribuicao)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var tenant = _db.Tenant.FirstOrDefault(t => t.Id == tenantId);
            var document = new RelatorioTemploCentral(
                relatorioContribuicao,
                tenant?.Nome ?? "Tenant",
                tenant?.Endereco ?? string.Empty,
                tenant?.PastorCongregacional ?? string.Empty);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}