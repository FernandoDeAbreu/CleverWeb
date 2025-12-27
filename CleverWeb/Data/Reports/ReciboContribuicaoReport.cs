using CleverWeb.Models;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;


namespace CleverWeb.Data.Reports
{
    public class ReciboContribuicaoReport(Contribuicao contribuicao) : IDocument
    {
        private readonly Contribuicao _contribuicao = contribuicao;

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
                page.Footer().AlignCenter().Text("Documento gerado automaticamente");
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Text("RECIBO DE CONTRIBUIÇÃO")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                col.Item().PaddingTop(10)
                    .LineHorizontal(1);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(20).Column(col =>
            {
                col.Spacing(10);

                col.Item().Text($"Membro: {_contribuicao.Membro?.Nome}");
                col.Item().Text($"Tipo de Contribuição: {_contribuicao.TipoContribuicao}");
                col.Item().Text($"Forma de Pagamento: {_contribuicao.FormaPagto}");
                col.Item().Text($"Valor: {_contribuicao.Valor:C}");
                col.Item().Text($"Data do Pagamento: {_contribuicao.DataPagamento:dd/MM/yyyy}");

                col.Item().PaddingTop(20)
                    .Text("Declaro que recebi a contribuição acima descrita.")
                    .Italic();
            });
        }
    }
}
