using System.ComponentModel.DataAnnotations;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Caixa.ViewModels
{
    public class MovimentoCaixaViewModel
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public FormaPagto FormaPagto { get; set; }
        public TipoContribuicao? TipoContribuicao { get; set; } 
        public string Tipo { get; set; } // Entrada | Saída
        public string Origem { get; set; } // Membro | Fornecedor
        public int CaixaID { get; set; }
    }
}