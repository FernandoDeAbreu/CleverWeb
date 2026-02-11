using CleverWeb.Data.Reports.Models;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace CleverWeb.Data.Reports
{
    public class RelatorioTemploCentralMovimentoCaixaDizimo(RelatorioMovimentoCaixaViewModel ralatorio, Caixa caixa) : IDocument
    {
        private readonly RelatorioMovimentoCaixaViewModel _ralatorio = ralatorio;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
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
                     .Text($" Movimento do caixa de {_ralatorio.Filtro.TipoContribuicao} no período {caixa.DtInicial:dd/MM/yyyy} à {caixa.DtFinal:dd/MM/yyyy} ").FontSize(12).Bold().ParagraphSpacing(10);
                    col.Item()
                     .BorderRight(0.5f)
                     .BorderLeft(0.5f).Text(" Congregação: IEADA SHEKINAH | Endereço: Rua Goias - Centro - Açailândia-MA").FontSize(12);
                    col.Item()
                     .BorderRight(0.5f)
                     .BorderLeft(0.5f)
                     .BorderBottom(0.5f)
                     .Text("");
                });
        }

        private void ComposeContent(IContainer container)
        {
            var totais = CalcularTotal();
            container.Table(table =>
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
                        header.Cell().Border(0.5f).Text("MOVIMENTO DO CAIXA DE DÍZIMOS").Bold().AlignCenter(); ;
                        header.Cell().Border(0.5f).Text("ENTRADAS").Bold().AlignCenter(); ;
                    });

                    // Linhas (exemplo)
                    foreach (var item in _ralatorio.Lista)
                    {
                        if (item.Valor > 0)
                        {
                            table.Cell().Border(0.5f).Text($" {item.Data:dd/MM/yyyy}");
                            table.Cell().Border(0.5f).Text($" {item.Origem}");
                            table.Cell().Border(0.5f).Text($"{item.Valor.ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();
                        }
                    }

                    // Total entradas
                    table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).Text($" ");

                    table.Cell().ColumnSpan(3)
                    .BorderLeft(0.5f)
                    .BorderRight(0.5f)
                    .Text($" Total de entradas  {totais.TotalEntradas.ToString("C", new CultureInfo("pt-BR"))}").Bold().AlignRight();
                    table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).BorderBottom(0.5f).Text($" ");

                    table.Cell().ColumnSpan(3).Text($" ");
                    table.Cell().ColumnSpan(3).Text($" ");

                    // Despesas
                    table.Cell().ColumnSpan(3).Border(0.5f).Text("DESPESAS E SAÍDAS").Bold().AlignCenter();

                    // Linhas
                    foreach (var item in _ralatorio.Lista)
                    {
                        if (item.Valor < 0)
                        {
                            table.Cell().ColumnSpan(2).Border(0.5f).Text($" {item.Descricao}");
                            table.Cell().Border(0.5f).Text($"{item.Valor.ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();
                        }
                    }
                    // Total Despesas

                    table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).Text($" ");

                    table.Cell().ColumnSpan(3)
                    .BorderLeft(0.5f)
                    .BorderRight(0.5f)
                    .Text($" Total de saídas  {_ralatorio.Lista.Where(m => m.Valor < 0).Sum(m => m.Valor).ToString("C", new CultureInfo("pt-BR"))}").Bold().AlignRight();
                    table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).BorderBottom(0.5f).Text($" ");
                });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.Table(table =>
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
        }

        private TotalMovCaixaModel CalcularTotal()
        {
            var totalEntradas = _ralatorio.Lista.Where(m => m.Valor > 0).Sum(m => m.Valor);
            var saida10 = _ralatorio.Lista.Where(m => m.Valor > 0).Sum(m => m.Valor * -1) * 10 / 100;
            var saida30 = _ralatorio.Lista.Where(m => m.Valor > 0).Sum(m => m.Valor * -1) * 30 / 100;
            var saida40 = _ralatorio.Lista.Where(m => m.Valor > 0).Sum(m => m.Valor * -1) * 40 / 100;

            var totalSaidasFixas = saida10 + saida30 + saida40;

            var totalSaidas = _ralatorio.Lista.Where(m => m.Valor < 0).Sum(m => m.Valor) + totalSaidasFixas;

            return new TotalMovCaixaModel
            {
                TotalEntradas = totalEntradas,
                Saida10 = saida10,
                Saida30 = saida30,
                Saida40 = saida40,
                TotalSaidasFixas = totalSaidasFixas,
                TotalSaidas = totalSaidas,
            };
        }
    }
}