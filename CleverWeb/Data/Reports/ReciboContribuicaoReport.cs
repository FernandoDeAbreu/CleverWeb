using CleverWeb.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


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
                page.Footer().AlignCenter().BorderBottom(0.5f);
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
                 .Text("IGREJA EVANGÉLICA ASSEMBLEIA DE DEUS - SETA").FontSize(18).Bold().AlignCenter();
                col.Item()
                  .BorderRight(0.5f)
                  .BorderLeft(0.5f).Text($"{_contribuicao.Id}").FontSize(12).Italic().AlignRight();
                col.Item()
                 .BorderRight(0.5f)
                 .BorderLeft(0.5f).Text(" Congregação: IEADA SHEKINAH ").FontSize(12);
                col.Item()
                 .BorderRight(0.5f)
                 .BorderLeft(0.5f).Text(" Endereço: Rua Goias 1634 - Centro - Açailândia-MA").FontSize(12);
                col.Item()
                 .BorderRight(0.5f)
                 .BorderLeft(0.5f)
                 .Text("");
                col.Item()
                 .BorderBottom(0.5f)
                 .BorderLeft(0.5f)
                 .BorderRight(0.5f)
                 .Text("COMADESMA FILIADA À CGADB").FontSize(14).Bold().AlignCenter();
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(20).Column(col =>
            {
                col.Spacing(10);

                col.Item().Text("RECIBO DE CONTRIBUIÇÃO VOLUNTÁRIA").Bold().AlignCenter(); 

                col.Spacing(5);

                col.Item().Text($"Membro: {_contribuicao.Membro?.Nome}");
                col.Item().Text($"Tipo de Contribuição: {_contribuicao.TipoContribuicao}");
                col.Item().Text($"Forma de Pagamento: {_contribuicao.FormaPagto}");
                col.Item().Text($"Valor: {_contribuicao.Valor:C}");
                col.Item().Text($"Data do Pagamento: {_contribuicao.DataPagamento:dd/MM/yyyy}");

                col.Item().PaddingTop(20)
                    .Text("Viva sob as promessas de malaquias 3.10")
                    .Italic();

                col.Spacing(20);
                col.Item().BorderTop(0.5f).Text("Pr. Fernando de Abreu Silva").AlignCenter()
                   .Italic();

            });
        }
    }
}
