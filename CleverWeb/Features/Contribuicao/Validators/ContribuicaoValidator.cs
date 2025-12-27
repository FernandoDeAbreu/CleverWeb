using CleverWeb.Features.Contribuicao.ViewModels;
using FluentValidation;

namespace CleverWeb.Features.Contribuicao.Validators
{
    public class ContribuicaoValidator : AbstractValidator<ContribuicaoViewModel>
    {
        public ContribuicaoValidator()
        {
            RuleFor(x => x.Valor)
                .NotEmpty();
          
        }
    }
}