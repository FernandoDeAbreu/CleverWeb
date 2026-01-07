using AutoMapper;
using CleverWeb.Features.Caixa.ViewModels;

namespace CleverWeb.Features.Caixa.Mapping
{
    public class CaixaProfile : Profile
    {
        public CaixaProfile()
        {
            CreateMap<Models.Caixa, CaixaViewModel>().ReverseMap();
            CreateMap<Models.Contribuicao, MovimentoCaixaViewModel>().ReverseMap();
        }
    }
}
