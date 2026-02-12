using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Features.Despesa.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Features.Despesa.Services
{
    public class DespesaService
    {
        private readonly CleverDbContext _db;
        private readonly IMapper _mapper;

        public DespesaService(CleverDbContext cleverDbContext, IMapper mapper)
        {
            _db = cleverDbContext;
            _mapper = mapper;
        }

        public async Task<DespesaViewModel> Create()
        {
            var model = new DespesaViewModel
            {
                FornecedorList = await _db.Fornecedor
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
            _db.Despesa.Add(entidade);
            await _db.SaveChangesAsync();
        }
    }
}