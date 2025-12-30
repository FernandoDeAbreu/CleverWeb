using AutoMapper;
using CleverWeb.Features.Despesa.ViewModels;

namespace CleverWeb.Features.Despesa.Mapping
{
    public class DespesaProfile : Profile
    {
        public DespesaProfile()
        {
            CreateMap<Models.Despesa, DespesaViewModel>().ReverseMap();
        }
    }
}