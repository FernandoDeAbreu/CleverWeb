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
            var contribuicoesQuery = _context.Contribuicao.Where(c => c.CaixaID == filtro.CaixaId).Include(c => c.Membro)
                                                .Select(c => new
                                                {
                                                    Id = c.Id,
                                                    Data = c.DataPagamento,
                                                    Descricao = "Contribuição - " + c.TipoContribuicao,
                                                    Valor = c.Valor,
                                                    Tipo = "Entrada",
                                                    TipoContribuicao = c.TipoContribuicao,
                                                    Origem = c.Membro.Nome,
                                                });

            var despesasQuery = _context.Despesa.Where(c => c.CaixaId == filtro.CaixaId).Include(d => d.Fornecedor)
                                                 .Select(d => new
                                                 {
                                                     Id = d.Id,
                                                     Data = d.DataPagamento,
                                                     Descricao = d.Descricao,
                                                     Valor = d.Valor * -1,
                                                     Tipo = "Saída",
                                                     TipoContribuicao = d.CaixaSaida,
                                                     Origem = d.Fornecedor.Descricao,
                                                 });

            var movimentoCaixa = contribuicoesQuery
                                              .Union(despesasQuery)
                                              .Select(m => new MovimentoCaixaViewModel
                                              {
                                                  Id = m.Id,
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

        public async Task<List<ViewModels.CaixaViewModel>> HistoricoCaixa()
        {
            var caixa = await _context.Caixa.OrderByDescending(c => c.DtFechamento).ToListAsync();

            return _mapper.Map<List<CaixaViewModel>>(caixa);
        }

        public async Task FecharCaixa(RelatorioMovimentoCaixaViewModel relatorio)
        {
            if (relatorio?.Lista == null || !relatorio.Lista.Any())
                throw new InvalidOperationException("Não há movimentos para fechar o caixa.");

            if(relatorio?.Lista.Where(c => c.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo) == null)
                throw new InvalidOperationException("O movimento selecionado não é de Dizimos.");


            var caixa = await CriarNovoCaixa(relatorio);

            await FecharCaixaDespesa(relatorio, caixa);

            await FecharCaixaContribuicao(relatorio, caixa);

        }

        private async Task FecharCaixaContribuicao(RelatorioMovimentoCaixaViewModel relatorioContribuicao, Models.Caixa caixa)
        {
            var contribuicoes = relatorioContribuicao.Lista.Where(c => c.Tipo == "Entrada");

            await SaidaParaConvencao10(contribuicoes, caixa);

            await SaidaParaConvencao40(contribuicoes, caixa);

            await SaidaParaConvencao30(contribuicoes, caixa);

            foreach (var item in contribuicoes)
            {
                var contribuica = await _context.Contribuicao.FirstOrDefaultAsync(c => c.Id == item.Id);
                contribuica.CaixaID = caixa.Id;
                _context.Contribuicao.Update(contribuica);
            }
            await _context.SaveChangesAsync();
        }

        private async Task SaidaParaConvencao10(IEnumerable<MovimentoCaixaViewModel> movimentoCaixas, Models.Caixa caixa)
        {
            var dizimos = movimentoCaixas.Where(c => c.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo);

            var valor = dizimos.Sum(c => c.Valor) * 10 / 100;

            var depesa = new Models.Despesa
            {
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Dízimo,
                Descricao = "10% - Saída para convenção",
                Valor = valor,
                FornecedorId = 9
            };
            await _context.Despesa.AddAsync(depesa);
            await _context.SaveChangesAsync();
        }

        private async Task SaidaParaConvencao40(IEnumerable<MovimentoCaixaViewModel> movimentoCaixas, Models.Caixa caixa)
        {
            var dizimos = movimentoCaixas.Where(c => c.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo);

            var valor = dizimos.Sum(c => c.Valor) * 40 / 100;

            var depesa = new Models.Despesa
            {
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Dízimo,
                Descricao = "40% - Saída para sede",
                Valor = valor,
                FornecedorId = 9
            };
            await _context.Despesa.AddAsync(depesa);
            await _context.SaveChangesAsync();
        }

        private async Task SaidaParaConvencao30(IEnumerable<MovimentoCaixaViewModel> movimentoCaixas, Models.Caixa caixa)
        {
            var dizimos = movimentoCaixas.Where(c => c.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo);

            var valor = dizimos.Sum(c => c.Valor) * 30 / 100;

            var depesa = new Models.Despesa
            {
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Dízimo,
                Descricao = "30% - Auxílo eclesiástico",
                Valor = valor,
                FornecedorId = 9
            };
            await _context.Despesa.AddAsync(depesa);
            await _context.SaveChangesAsync();
        }

        private async Task FecharCaixaDespesa(RelatorioMovimentoCaixaViewModel relatorioContribuicao, Models.Caixa caixa)
        {
            var despesas = relatorioContribuicao.Lista.Where(c => c.Tipo == "Saída");

            foreach (var item in despesas)
            {
                var despesa = await _context.Despesa.FirstOrDefaultAsync(c => c.Id == item.Id);
                despesa.CaixaId = caixa.Id;
                _context.Despesa.Update(despesa);
            }
            await _context.SaveChangesAsync();
        }

        private async Task<Models.Caixa> CriarNovoCaixa(RelatorioMovimentoCaixaViewModel relatorio)
        {

            var totalDizimo = relatorio.Lista.Where(c => c.Tipo == "Entrada").Sum(c => c.Valor);

            var saldoDizimo = totalDizimo - (totalDizimo * 80 / 100); // Saldo do dizimo é 20% do total, pois 80% é destinado para SEDE

            var totalDespesas = relatorio.Lista.Where(c => c.Tipo == "Saída").Sum(c => c.Valor) * -1;

            var ultimoCaixa = await _context.Caixa
                              .OrderByDescending(x => x.Id)
                              .FirstOrDefaultAsync();

            var caixa = new Models.Caixa
            {
                DtFechamento = DateTime.Now,
                DtInicial = relatorio.Filtro.DataInicio ?? DateTime.Now,
                DtFinal = relatorio.Filtro.DataFim ?? DateTime.Now,
                Saldo = (saldoDizimo - totalDespesas) + ultimoCaixa.Saldo,
                UsuarioId = 1,
            };

            _context.Caixa.Add(caixa);

            await _context.SaveChangesAsync();

            return caixa;
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

        private byte[] RelatorioTemploCentralMovimentoCaixaDizimo(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var caixa = _context.Caixa.FirstOrDefault(c => c.Id == relatorioContribuicao.Filtro.CaixaId);

            var document = new RelatorioTemploCentralMovimentoCaixaDizimo(relatorioContribuicao, caixa);

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