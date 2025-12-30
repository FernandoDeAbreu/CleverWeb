using CleverWeb.Features.Despesa.ViewModels;
using FluentValidation;

namespace CleverWeb.Features.Despesa.Validators
{
    public class DespesaValidator : AbstractValidator<DespesaViewModel>
    {
        public DespesaValidator()
        {
            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(35);
   
        }
    }
}