using AutoMapper;
using CleverWeb.Features.Caixa.ViewModels;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Features.Membro.ViewModels;

namespace CleverWeb.Features.Contribuicao.Mapping
{
    public class ContribuicaoProfile : Profile
    {
        public ContribuicaoProfile()
        {
            CreateMap<Models.Contribuicao, ContribuicaoViewModel>().ReverseMap();
            CreateMap<Models.Contribuicao, MovimentoCaixaViewModel>().ReverseMap();

        }
    }
}
