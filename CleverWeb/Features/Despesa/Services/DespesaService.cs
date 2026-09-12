using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Despesa.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Features.Despesa.Services
{
    public class DespesaService
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;
        private readonly CaixaService _caixaService;

        public DespesaService(CleverDbContext cleverDbContext, IMapper mapper, ITenantAccessor tenantAccessor, CaixaService caixaService)
        {
            _db = cleverDbContext;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
            _caixaService = caixaService;
        }

        public async Task<DespesaViewModel> Create()
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var model = new DespesaViewModel
            {
                FornecedorList = await _db.Fornecedor
                    .Where(f => f.TenantId == tenantId)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Descricao
                    })
                    .ToListAsync()
            };

            return model;
        }

        public async Task Create(DespesaViewModel model)
        {
            var entidade = _mapper.Map<Models.Despesa>(model);
            entidade.TenantId = _tenantAccessor.CurrentTenantId ?? 0;
            _db.Despesa.Add(entidade);
            await _db.SaveChangesAsync();
            await _caixaService.AtualizarSaldoAtual(entidade.CaixaSaida, -entidade.Valor);
        }

        public async Task Estornar(int id, string? motivo)
        {
            var motivoNormalizado = motivo?.Trim();
            if (string.IsNullOrWhiteSpace(motivoNormalizado) || motivoNormalizado.Length < 15)
                throw new InvalidOperationException("O motivo do estorno deve ser informado e conter no mínimo 15 caracteres.");

            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var despesa = await _db.Despesa
                .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);

            if (despesa == null)
                throw new InvalidOperationException("Despesa não encontrada.");

            if (!string.IsNullOrWhiteSpace(despesa.MotivoExclusao))
                throw new InvalidOperationException("Esta despesa já foi estornada.");

            if (despesa.GeradaNoFechamento)
                throw new InvalidOperationException("Despesas geradas pelo fechamento devem ser estornadas pelo cancelamento do fechamento.");

            if (despesa.CaixaId > 0)
                throw new InvalidOperationException("Não é possível estornar uma despesa vinculada a um caixa já fechado. Cancele o fechamento do caixa primeiro.");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            var caixa = despesa.CaixaId > 0
                ? await _db.Caixa.FirstOrDefaultAsync(c => c.Id == despesa.CaixaId && c.TenantId == tenantId)
                : await _db.Caixa
                    .Where(c => c.TenantId == tenantId
                        && c.TipoContribuicao == despesa.CaixaSaida
                        && !c.Cancelado)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefaultAsync();

            if (caixa != null && !caixa.Cancelado)
                caixa.SaldoAtual += despesa.Valor;

            despesa.MotivoExclusao = motivoNormalizado;
            despesa.DataExclusao = DateTime.Now;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}