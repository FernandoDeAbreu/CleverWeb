using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Contribuicao.ViewModels
{
     public class FiltroContribuicaoViewModel
 {
     public string? NomeMembro { get; set; }
     public TipoContribuicao? TipoContribuicao { get; set; }
     public FormaPagto? FormaPagto { get; set; }
     public DateTime? DataInicio { get; set; } = DateTime.Today.AddDays(-7);
     public DateTime? DataFim { get; set; } =  DateTime.Today;
 }
    public class RelatorioContribuicaoViewModel
    {
        public FiltroContribuicaoViewModel Filtro { get; set; } = new();
        public IEnumerable<ContribuicaoViewModel> Lista { get; set; } = Enumerable.Empty<ContribuicaoViewModel>();
        public decimal Total => Lista.Sum(x => x.Valor);
    }

}
