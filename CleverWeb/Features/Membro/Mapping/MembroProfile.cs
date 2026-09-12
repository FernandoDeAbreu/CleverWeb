using AutoMapper;
using CleverWeb.Features.Membro.ViewModels;

namespace CleverWeb.Features.Membro.Mapping
{
    public class MembroProfile : Profile
    {
        public MembroProfile()
        {
            CreateMap<Models.Membro, MembroViewModel>();
            CreateMap<MembroViewModel, Models.Membro>()
                .ForMember(dest => dest.TenantId, options => options.Ignore());
        }
    }
}
