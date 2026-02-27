using CleverWeb.Features.Caixa.ViewModels;

namespace CleverWeb.Features.Dashboard.ViewModels
{
    public class DashboardViewModel
    {
        public decimal SaldoCaixaDizimo { get; set; }
        public decimal SaldoCaixaOfertas { get; set; }
        public decimal SaldoCaixaMissao { get; set; }
        public IEnumerable<MovimentoCaixaViewModel> Lista { get; set; } = Enumerable.Empty<MovimentoCaixaViewModel>();
    }
}
