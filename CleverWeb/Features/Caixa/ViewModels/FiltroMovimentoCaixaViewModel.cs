using CleverWeb.Features.Caixa.ViewModels;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Contribuicao.ViewModels
{
    public class FiltroMovimentoCaixaViewModel
    {
        public string? NomeMembro { get; set; }
        public TipoContribuicao? TipoContribuicao { get; set; }
        public FormaPagto? FormaPagto { get; set; }
        public DateTime? DataInicio { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime? DataFim { get; set; } = DateTime.Today;
        public int CaixaId { get; set; }
    }

    public class RelatorioMovimentoCaixaViewModel
    {
        public FiltroMovimentoCaixaViewModel Filtro { get; set; } = new();
        public IEnumerable<MovimentoCaixaViewModel> Lista { get; set; } = Enumerable.Empty<MovimentoCaixaViewModel>();
        public CaixaViewModel Caixa { get; set; } = new();
        public decimal Total => Lista.Sum(x => x.Valor);
    }
}