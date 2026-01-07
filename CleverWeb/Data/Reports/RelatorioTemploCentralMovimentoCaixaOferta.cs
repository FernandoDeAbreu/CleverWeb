using CleverWeb.Features.Contribuicao.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace CleverWeb.Data.Reports
{
    public class RelatorioTemploCentralMovimentoCaixaOferta(RelatorioMovimentoCaixaViewModel ralatorio) : IDocument
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
                     .Text($" Movimento do caixa de {_ralatorio.Filtro.TipoContribuicao} no período {_ralatorio.Filtro.DataInicio:dd/MM/yyyy} à {_ralatorio.Filtro.DataFim:dd/MM/yyyy} ").FontSize(12).Bold().ParagraphSpacing(10);
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
                        header.Cell().Border(0.5f).Text("MOVIMENTO DO CAIXA DE OFERTAS").Bold().AlignCenter(); ;
                        header.Cell().Border(0.5f).Text("ENTRADAS").Bold().AlignCenter(); ;
                    });

                    // Linhas (exemplo)
                    foreach (var item in _ralatorio.Lista)
                    {
                        table.Cell().Border(0.5f).Text($" {item.Data:dd/MM/yyyy}");
                        table.Cell().Border(0.5f).Text($" {item.Origem}");
                        table.Cell().Border(0.5f).Text($"{item.Valor.ToString("C", new CultureInfo("pt-BR"))}  ").AlignEnd();
                    }

                    // Total entradas
                    table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).Text($" ");

                    table.Cell().ColumnSpan(3)
                    .BorderLeft(0.5f)
                    .BorderRight(0.5f)
                    .Text($" Total de entradas  {(_ralatorio.Total).ToString("C", new CultureInfo("pt-BR"))}").Bold().AlignRight();
                    table.Cell().ColumnSpan(3).BorderLeft(0.5f).BorderRight(0.5f).BorderBottom(0.5f).Text($" ");

                    table.Cell().ColumnSpan(3).Text($" ");
                    table.Cell().ColumnSpan(3).Text($" ");
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
    }
}