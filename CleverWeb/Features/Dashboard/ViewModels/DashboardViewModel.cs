using CleverWeb.Features.Caixa.ViewModels;

namespace CleverWeb.Features.Dashboard.ViewModels
{
    public class DashboardViewModel
    {
        public decimal SaldoCaixaDizimo { get; set; }
        public decimal SaldoCaixaOfertas { get; set; }
        public decimal SaldoCaixaMissao { get; set; }
        public int TotalMembros { get; set; }
        public int NovosMembrosNoMes { get; set; }
        public decimal EntradasUltimosSeteDias { get; set; }
        public decimal SaidasUltimosSeteDias { get; set; }
        public IReadOnlyList<string> Periodos { get; set; } = Array.Empty<string>();
        public IReadOnlyList<decimal> EntradasPorPeriodo { get; set; } = Array.Empty<decimal>();
        public IReadOnlyList<decimal> SaidasPorPeriodo { get; set; } = Array.Empty<decimal>();
        public IReadOnlyList<int> MembrosPorPeriodo { get; set; } = Array.Empty<int>();
        public IEnumerable<MovimentoCaixaViewModel> Lista { get; set; } = Enumerable.Empty<MovimentoCaixaViewModel>();
    }
}
