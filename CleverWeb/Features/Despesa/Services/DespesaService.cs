using AutoMapper;
using CleverWeb.Data;
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

        public DespesaService(CleverDbContext cleverDbContext, IMapper mapper, ITenantAccessor tenantAccessor)
        {
            _db = cleverDbContext;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
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
        }
    }
}