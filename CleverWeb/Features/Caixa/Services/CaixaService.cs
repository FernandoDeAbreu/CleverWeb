using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Data.Reports;
using CleverWeb.Features.Caixa.ViewModels;
using CleverWeb.Features.Contribuicao.ViewModels;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace CleverWeb.Features.Caixa.Services
{
    public class CaixaService
    {
        private readonly CleverDbContext _context;
        private readonly IMapper _mapper;

        public CaixaService(CleverDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public RelatorioMovimentoCaixaViewModel ObterRelatorio(FiltroMovimentoCaixaViewModel filtro)
        {
            var contribuicoesQuery = _context.Contribuicao.Where(c => c.CaixaID == 0).Include(c => c.Membro)
                                                .Select(c => new
                                                {
                                                    Data = c.DataPagamento,
                                                    Descricao = "Contribuição - " + c.TipoContribuicao,
                                                    Valor = c.Valor,
                                                    Tipo = "Entrada",
                                                    TipoContribuicao = c.TipoContribuicao,
                                                    Origem = c.Membro.Nome
                                                });

            var despesasQuery = _context.Despesa.Where(c => c.CaixaID == 0).Include(d => d.Fornecedor)
                                                 .Select(d => new
                                                 {
                                                     Data = d.DataPagamento,
                                                     Descricao = "Despesa - " + d.CaixaSaida,
                                                     Valor = d.Valor * -1,
                                                     Tipo = "Saída",
                                                     TipoContribuicao = d.CaixaSaida,
                                                     Origem = d.Fornecedor.Descricao
                                                 });

            var movimentoCaixa = contribuicoesQuery
                                              .Union(despesasQuery)
                                              .Select(m => new MovimentoCaixaViewModel
                                              {
                                                  Data = m.Data,
                                                  Descricao = m.Descricao,
                                                  Valor = m.Valor,
                                                  Tipo = m.Tipo,
                                                  TipoContribuicao = m.TipoContribuicao,
                                                  Origem = m.Origem
                                              })
                                              .OrderBy(m => m.Data).AsQueryable();

            if (filtro.TipoContribuicao.HasValue)
            {
                movimentoCaixa = movimentoCaixa.Where(x => x.TipoContribuicao == filtro.TipoContribuicao);
            }
            if (filtro.DataInicio.HasValue)
            {
                movimentoCaixa = movimentoCaixa.Where(x => x.Data >= filtro.DataInicio);
            }
            if (filtro.DataFim.HasValue)
            {
                movimentoCaixa = movimentoCaixa.Where(x => x.Data <= filtro.DataFim);
            }

            return new RelatorioMovimentoCaixaViewModel
            {
                Filtro = filtro,
                Lista = movimentoCaixa
            };
        }

        public byte[] ExportarPdf(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            if (relatorioContribuicao.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo)
                return RelatorioTemploCentralMovimentoCaixaDizimo(relatorioContribuicao);
            if (relatorioContribuicao.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Missão)
                return RelatorioTemploCentralMovimentoCaixaMissao(relatorioContribuicao);
            if (relatorioContribuicao.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Oferta)
                return RelatorioTemploCentralMovimentoCaixaOferta(relatorioContribuicao);

            return RelatorioGeral(relatorioContribuicao);
        }

        private static byte[] RelatorioGeral(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var document = new RelatorioTemploCentralMovimentoCaixa(relatorioContribuicao);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }

        private static byte[] RelatorioTemploCentralMovimentoCaixaDizimo(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var document = new RelatorioTemploCentralMovimentoCaixaDizimo(relatorioContribuicao);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }

        private static byte[] RelatorioTemploCentralMovimentoCaixaMissao(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var document = new RelatorioTemploCentralMovimentoCaixaMissao(relatorioContribuicao);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }

        private static byte[] RelatorioTemploCentralMovimentoCaixaOferta(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var document = new RelatorioTemploCentralMovimentoCaixaOferta(relatorioContribuicao);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}