using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleverWeb.Features.Membro.ViewModels
{
    public class MembroViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; set; } = DateTime.Today.AddDays(-1);

        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "Igreja")]
        public int TenantId { get; set; }

        public List<SelectListItem> TenantOptions { get; set; } = new();

        [Display(Name = "Data de cadastro")]
        public DateTime DataCadastro { get; set; }
    }
}