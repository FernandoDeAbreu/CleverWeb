using AutoMapper;
using CleverWeb.Features.Fornecedor.ViewModels;

namespace CleverWeb.Features.Fornecedor.Mapping
{
    public class FornecedorProfile : Profile
    {
        public FornecedorProfile()
        {
            CreateMap<Models.Fornecedor, FornecedorViewModel>().ReverseMap();
        }
    }
}
