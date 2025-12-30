using CleverWeb.Features.Membro.ViewModels;
using FluentValidation;

namespace CleverWeb.Features.Membro.Validators
{
    public class MembroValidator : AbstractValidator<MembroViewModel>
    {
        public MembroValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(25);
            RuleFor(x => x.DataNascimento)
                .LessThan(DateTime.Today).WithMessage("Data de nascimento inválida");
            RuleFor(x => x.Telefone)
                .MaximumLength(50);
        }
    }
}
