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

        public RelatorioMovimentoCaixaViewModel ObterDados(int id)
        {
            var caixa = _context.Caixa.FirstOrDefault(c => c.Id == id);

            var filtro = new FiltroMovimentoCaixaViewModel
            {
                CaixaId = id,
                TipoContribuicao = caixa?.TipoContribuicao,
                DataFim = null,
                DataInicio = null,
            };
            return ObterRelatorio(filtro);
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

            var caixa = _context.Caixa.FirstOrDefault(c => c.Id == filtro.CaixaId);

            return new RelatorioMovimentoCaixaViewModel
            {
                Filtro = filtro,
                Lista = movimentoCaixa,
                Caixa = caixa != null ? _mapper.Map<CaixaViewModel>(caixa) : new CaixaViewModel()
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

            if (relatorio.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Oferta)
                await FecharCaixaOferta(relatorio);
            if (relatorio.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo)
                await FecharCaixaDizimo(relatorio);
            if (relatorio.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Missão)
                await FecharCaixaMissao(relatorio);
        }

        private async Task FecharCaixaOferta(RelatorioMovimentoCaixaViewModel relatorio)
        {
            var receitas = relatorio.Lista.Where(c => c.Tipo == "Entrada");

            var caixa = await CriarNovoCaixa(relatorio, receitas.Sum(c => c.Valor));

            await AtualizarDespesa(relatorio, caixa);

            foreach (var item in receitas)
            {
                var contribuica = await _context.Contribuicao.FirstOrDefaultAsync(c => c.Id == item.Id);
                contribuica.CaixaID = caixa.Id;
                _context.Contribuicao.Update(contribuica);
            }
            await _context.SaveChangesAsync();
        }

        private async Task FecharCaixaMissao(RelatorioMovimentoCaixaViewModel relatorio)
        {
            var receitas = relatorio.Lista.Where(c => c.Tipo == "Entrada");

            var caixa = await CriarNovoCaixa(relatorio, receitas.Sum(c => c.Valor));

            await SaidaParaSecretariaMissao(receitas, caixa);

            foreach (var item in receitas)
            {
                var contribuica = await _context.Contribuicao.FirstOrDefaultAsync(c => c.Id == item.Id);
                contribuica.CaixaID = caixa.Id;
                _context.Contribuicao.Update(contribuica);
            }
            await _context.SaveChangesAsync();
        }

        private async Task FecharCaixaDizimo(RelatorioMovimentoCaixaViewModel relatorio)
        {
            var receitas = relatorio.Lista.Where(c => c.Tipo == "Entrada");

            var totalReceitas = receitas.Sum(c => c.Valor);

            var saldoDizimo = totalReceitas - (totalReceitas * 80 / 100); // Saldo do dizimo é 20% do total, pois 80% é destinado para SEDE

            var caixa = await CriarNovoCaixa(relatorio, saldoDizimo);

            await SaidaParaConvencao10(receitas, caixa);

            await SaidaParaConvencao40(receitas, caixa);

            await SaidaParaConvencao30(receitas, caixa);

            await AtualizarDespesa(relatorio, caixa);

            foreach (var item in receitas)
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

        private async Task SaidaParaSecretariaMissao(IEnumerable<MovimentoCaixaViewModel> movimentoCaixas, Models.Caixa caixa)
        {
            var missao = movimentoCaixas.Where(c => c.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Missão);

            var valor = missao.Sum(c => c.Valor);

            var depesa = new Models.Despesa
            {
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Missão,
                Descricao = "Secretaria de missões IEADA",
                Valor = valor,
                FornecedorId = 9
            };
            await _context.Despesa.AddAsync(depesa);
            await _context.SaveChangesAsync();
        }

        private async Task AtualizarDespesa(RelatorioMovimentoCaixaViewModel relatorio, Models.Caixa caixa)
        {
            var despesas = relatorio.Lista.Where(c => c.Tipo == "Saída");

            foreach (var item in despesas)
            {
                var despesa = await _context.Despesa.FirstOrDefaultAsync(c => c.Id == item.Id);
                despesa.CaixaId = caixa.Id;
                _context.Despesa.Update(despesa);
            }
            await _context.SaveChangesAsync();
        }

        private async Task<Models.Caixa> CriarNovoCaixa(RelatorioMovimentoCaixaViewModel relatorio, Decimal saldoReceita)
        {
            var despesas = relatorio.Lista.Where(c => c.Tipo == "Saída").Sum(c => c.Valor) * -1;

            var ultimoCaixa = await _context.Caixa.Where(c => c.TipoContribuicao == relatorio.Filtro.TipoContribuicao)
                              .OrderByDescending(x => x.Id)
                              .FirstOrDefaultAsync() ?? new Models.Caixa();

            var caixa = new Models.Caixa
            {
                DtFechamento = DateTime.Now,
                DtInicial = relatorio.Filtro.DataInicio ?? DateTime.Now,
                DtFinal = relatorio.Filtro.DataFim ?? DateTime.Now,
                SaldoAtual = (saldoReceita - despesas) + ultimoCaixa.SaldoAtual,
                SaldoAnterior = ultimoCaixa.SaldoAtual,
                TipoContribuicao = relatorio.Filtro.TipoContribuicao,
                UsuarioId = 1,
            };

            _context.Caixa.Add(caixa);

            await _context.SaveChangesAsync();

            return caixa;
        }

        public byte[] ExportarPdf(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var document = new RelatorioTemploCentralMovimentoCaixa(relatorioContribuicao);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}