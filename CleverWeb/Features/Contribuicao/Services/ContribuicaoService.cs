using CleverWeb.Data;
using CleverWeb.Features.Contribuicao.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Globalization;

namespace CleverWeb.Features.Contribuicao.Services
{
    public class ContribuicaoService
    {
        private readonly CleverDbContext _db;

        public ContribuicaoService(CleverDbContext db)
        {
            _db = db;
        }

        public IEnumerable<ContribuicaoViewModel> Relatorio(FiltroContribuicaoViewModel filtro)
        {
            var query = _db.Contribuicao.Include(c => c.Membro).OrderByDescending(c => c.DataPagamento).AsQueryable();

            if (!string.IsNullOrEmpty(filtro.NomeMembro))
                query = query.Where(c => c.Membro.Nome.Contains(filtro.NomeMembro));

            if (filtro.TipoContribuicao.HasValue)
                query = query.Where(c => c.TipoContribuicao == filtro.TipoContribuicao.Value);

            if (filtro.FormaPagto.HasValue)
                query = query.Where(c => c.FormaPagto == filtro.FormaPagto.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(c => c.DataPagamento >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(c => c.DataPagamento <= filtro.DataFim.Value);

            var lista = query.Select(c => new ContribuicaoViewModel
            {
                Id = c.Id,
                Membro = c.Membro,
                TipoContribuicao = c.TipoContribuicao,
                FormaPagto = c.FormaPagto,
                DataPagamento = c.DataPagamento,
                Valor = c.Valor
            }).ToList();

            return lista;
        }

        public async Task<byte[]> ExportarPdf(FiltroContribuicaoViewModel filtro)
        {
            var lista = await _db.Contribuicao.Include(c => c.Membro).OrderBy(c => c.DataPagamento).ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // Cabeçalho
                    page.Header()
                        .Column(col =>
                        {
                            col.Item()
                             .BorderTop(0.5f)
                             .BorderLeft(0.5f)
                             .BorderRight(0.5f)
                             .Text("IGREJA EVANGÉLICA ASSEMBLEIA DE DEUS - SETA").FontSize(20).Bold().AlignCenter();
                            col.Item()
                             .BorderRight(0.5f)
                             .BorderLeft(0.5f)
                             .Text("BR 222 - Km. 03 - Vila Pregresso II | CEP: 65.930-000 - Açailândia - Maranhão").FontSize(12).AlignCenter();
                            col.Item()
                             .BorderRight(0.5f)
                             .BorderLeft(0.5f)
                             .Text("");
                            col.Item()
                             .BorderTop(0.5f)
                             .BorderRight(0.5f)
                             .BorderLeft(0.5f)
                             .Text($" Movimento do caixa no perído {DateTime.Parse(filtro.DataInicio.ToString()).ToShortDateString()} à " +
                                                                $"{DateTime.Parse(filtro.DataInicio.ToString()).ToShortDateString()}").FontSize(12).Bold().ParagraphSpacing(10);
                            col.Item()
                             .BorderRight(0.5f)
                             .BorderLeft(0.5f).Text(" Congregação: IEADA SHEKINAH | Endereço: Rua Goias - Centro - Açailândia-MA").FontSize(12);
                            col.Item()
                             .BorderRight(0.5f)
                             .BorderLeft(0.5f)
                             .BorderBottom(0.5f)
                             .Text("");
                        });

                    // Conteúdo principal: tabela
                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Data
                                columns.RelativeColumn(5); // Descrição
                                columns.RelativeColumn(1); // Valor
                            });

                            // Cabeçalho da tabela
                            table.Header(header =>
                            {
                                header.Cell().Border(0.5f).Text("DATA").Bold().AlignCenter(); ;
                                header.Cell().Border(0.5f).Text("DÍZIMOS E OFERTAS").Bold().AlignCenter(); ;
                                header.Cell().Border(0.5f).Text("ENTRADAS").Bold().AlignCenter(); ;
                            });

                            // Linhas (exemplo)
                            foreach (var item in lista)
                            {
                                table.Cell().Border(0.5f).Text($" {item.DataPagamento.ToString("dd/MM/yyyy")}");
                                table.Cell().Border(0.5f).Text($" {item.Membro.Nome}");
                                table.Cell().Border(0.5f).Text($"{item.Valor.ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();
                            }



                            // Total entradas
                            table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).Text($" ");

                            table.Cell().ColumnSpan(3)
                            .BorderLeft(0.5f)
                            .BorderRight(0.5f)
                            .Text($" Total de entradas  {lista.Sum(x => x.Valor).ToString("C", new CultureInfo("pt-BR"))}").Bold().AlignRight();
                            table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).BorderBottom(0.5f).Text($" ");

                            table.Cell().ColumnSpan(3).Text($" ");
                            table.Cell().ColumnSpan(3).Text($" ");

                            // Despesas
                            table.Cell().ColumnSpan(3).Border(0.5f).Text($"DESPESAS E SAÍDAS").Bold().AlignCenter();

                            table.Cell().ColumnSpan(2).Border(0.5f).Text($" 10% - Saída para convenção");
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"{lista.Sum(x => x.Valor * 10/100).ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();

                            table.Cell().ColumnSpan(2).Border(0.5f).Text($" 40% - Saída para sede");
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"{lista.Sum(x => x.Valor * 40 / 100).ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();

                            table.Cell().ColumnSpan(2).Border(0.5f).Text($" 30% - Auxílo eclesiástico");
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"{lista.Sum(x => x.Valor * 30 / 100).ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();

                            table.Cell().ColumnSpan(2).Border(0.5f).Text($" 20% - Congregação");
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"{lista.Sum(x => x.Valor * 20 / 100).ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();

                            // Total Despesas

                            table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).Text($" ");

                            table.Cell().ColumnSpan(3)
                            .BorderLeft(0.5f)
                            .BorderRight(0.5f)
                            .Text($" Total de saídas  {lista.Sum(x => x.Valor).ToString("C", new CultureInfo("pt-BR"))}").Bold().AlignRight();
                            table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).BorderBottom(0.5f).Text($" ");

                        });

                    // Rodapé
                    page.Footer()
                        .Table(table =>
                        {

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Data
                                columns.RelativeColumn(1); // Descrição
                            });

                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"  Pastor congregacional"); 
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"  Visto");
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"  Tesoureiro");
                            table.Cell().ColumnSpan(1).Border(0.5f).Text($"  Visto");
                            table.Cell().ColumnSpan(2).Border(0.5f).Text($"  Recebido em:        /        /     ");

                        });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return pdfBytes;
        }
    }
}