using AutoMapper;
using CleverWeb.Data;
using CleverWeb.Data.Reports;
using CleverWeb.Features.Caixa.ViewModels;
using CleverWeb.Features.Contribuicao.ViewModels;
using CleverWeb.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace CleverWeb.Features.Caixa.Services
{
    public class CaixaService
    {
        private readonly CleverDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITenantAccessor _tenantAccessor;

        public CaixaService(CleverDbContext context, IMapper mapper, ITenantAccessor tenantAccessor)
        {
            _context = context;
            _mapper = mapper;
            _tenantAccessor = tenantAccessor;
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
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var contribuicoesQuery = _context.Contribuicao.Where(c => c.CaixaID == filtro.CaixaId && c.MotivoExclusao == null && c.TenantId == tenantId)
                                                .Include(c => c.Membro)
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

            var despesasQuery = _context.Despesa.Where(c => c.CaixaId == filtro.CaixaId && c.TenantId == tenantId).Include(d => d.Fornecedor)
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
                                              .OrderBy(m => m.Descricao).AsQueryable();

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

            var caixa = _context.Caixa.FirstOrDefault(c => c.Id == filtro.CaixaId && c.TenantId == tenantId);

            return new RelatorioMovimentoCaixaViewModel
            {
                Filtro = filtro,
                Lista = movimentoCaixa,
                Caixa = caixa != null ? _mapper.Map<CaixaViewModel>(caixa) : new CaixaViewModel()
            };
        }

        public async Task<List<ViewModels.CaixaViewModel>> HistoricoCaixa(DateTime? dataInicio = null, DateTime? dataFim = null, Data.Shared.Enums.TipoContribuicao? tipoContribuicao = null)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;

            var query = _context.Caixa.Where(c => c.TenantId == tenantId).AsQueryable();

            if (tipoContribuicao.HasValue)
            {
                query = query.Where(c => c.TipoContribuicao == tipoContribuicao.Value);
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DtFechamento >= dataInicio.Value.Date);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(c => c.DtFechamento <= dataFim.Value.Date);
            }

            var caixa = await query.OrderByDescending(c => c.DtFechamento).ToListAsync();

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

        public async Task AtualizarSaldoAtual(Data.Shared.Enums.TipoContribuicao tipo, decimal variacao)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var caixaAtual = await _context.Caixa
                .Where(c => c.TenantId == tenantId
                    && c.TipoContribuicao == tipo
                    && !c.Cancelado)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (caixaAtual == null)
                return;

            caixaAtual.SaldoAtual += variacao;
            await _context.SaveChangesAsync();
        }

        public async Task CancelarFechamento(int id, string? motivoCancelamento, int? usuarioId)
        {
            var motivo = motivoCancelamento?.Trim();
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 15)
                throw new InvalidOperationException("O motivo do cancelamento deve ser informado e conter no mínimo 15 caracteres.");

            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var caixa = await _context.Caixa
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

            if (caixa == null)
                throw new InvalidOperationException("Fechamento não encontrado.");

            if (caixa.Cancelado)
                throw new InvalidOperationException("Este fechamento já foi cancelado.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var valorEstornado = caixa.SaldoAtual - caixa.SaldoAnterior;

            var contribuicoes = await _context.Contribuicao
                .Where(c => c.CaixaID == id && c.TenantId == tenantId)
                .ToListAsync();

            foreach (var contribuicao in contribuicoes)
                contribuicao.CaixaID = 0;

            var despesas = await _context.Despesa
                .Where(d => d.CaixaId == id && d.TenantId == tenantId)
                .ToListAsync();

            var despesasAutomaticas = despesas.Where(EhDespesaAutomatica).ToList();
            _context.Despesa.RemoveRange(despesasAutomaticas);

            foreach (var despesa in despesas.Except(despesasAutomaticas))
                despesa.CaixaId = 0;

            caixa.SaldoAtual = caixa.SaldoAnterior;
            caixa.Cancelado = true;
            caixa.DtCancelamento = DateTime.Now;
            caixa.UsuarioCancelamentoId = usuarioId;
            caixa.MotivoCancelamento = motivo;

            if (valorEstornado != 0)
            {
                var fechamentosPosteriores = await _context.Caixa
                    .Where(c => c.TenantId == tenantId
                        && c.TipoContribuicao == caixa.TipoContribuicao
                        && c.Id > caixa.Id
                        && !c.Cancelado)
                    .ToListAsync();

                foreach (var fechamentoPosterior in fechamentosPosteriores)
                {
                    fechamentoPosterior.SaldoAnterior -= valorEstornado;
                    fechamentoPosterior.SaldoAtual -= valorEstornado;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private static bool EhDespesaAutomatica(Models.Despesa despesa)
        {
            return despesa.GeradaNoFechamento
                || despesa.Descricao.StartsWith("10% - Saída para convenção", StringComparison.OrdinalIgnoreCase)
                || despesa.Descricao.StartsWith("40% - Saída para sede", StringComparison.OrdinalIgnoreCase)
                || despesa.Descricao.StartsWith("30% - Auxílo eclesiástico", StringComparison.OrdinalIgnoreCase)
                || despesa.Descricao.StartsWith("Secretaria de missões IEADA", StringComparison.OrdinalIgnoreCase);
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
                TenantId = _tenantAccessor.CurrentTenantId ?? 0,
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Dízimo,
                Descricao = "10% - Saída para convenção",
                Valor = valor,
                FornecedorId = 9,
                GeradaNoFechamento = true
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
                TenantId = _tenantAccessor.CurrentTenantId ?? 0,
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Dízimo,
                Descricao = "40% - Saída para sede",
                Valor = valor,
                FornecedorId = 9,
                GeradaNoFechamento = true
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
                TenantId = _tenantAccessor.CurrentTenantId ?? 0,
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Dízimo,
                Descricao = "30% - Auxílo eclesiástico",
                Valor = valor,
                FornecedorId = 9,
                GeradaNoFechamento = true
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
                TenantId = _tenantAccessor.CurrentTenantId ?? 0,
                CaixaId = caixa.Id,
                DataPagamento = DateTime.Now,
                CaixaSaida = Data.Shared.Enums.TipoContribuicao.Missão,
                Descricao = "Secretaria de missões IEADA",
                Valor = valor,
                FornecedorId = 9,
                GeradaNoFechamento = true
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
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var despesas = relatorio.Lista.Where(c => c.Tipo == "Saída").Sum(c => c.Valor) * -1;

            var ultimoCaixa = await _context.Caixa.Where(c => c.TipoContribuicao == relatorio.Filtro.TipoContribuicao && c.TenantId == tenantId && !c.Cancelado)
                              .OrderByDescending(x => x.Id)
                              .FirstOrDefaultAsync() ?? new Models.Caixa();

            var saldoAtual = (saldoReceita - despesas) + ultimoCaixa.SaldoAtual;
            var saldoAnterior = ultimoCaixa.SaldoAtual;

            if (ultimoCaixa.Id > 0)
            {
                var variacaoLancamentos = relatorio.Lista.Sum(c => c.Valor);
                saldoAnterior = ultimoCaixa.SaldoAtual - variacaoLancamentos;
                saldoAtual = ultimoCaixa.SaldoAtual;

                if (relatorio.Filtro.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo)
                {
                    var dizimos = relatorio.Lista
                        .Where(c => c.Tipo == "Entrada" && c.TipoContribuicao == Data.Shared.Enums.TipoContribuicao.Dízimo)
                        .Sum(c => c.Valor);
                    saldoAtual -= dizimos * 80 / 100;
                }
            }

            var caixa = new Models.Caixa
            {
                TenantId = _tenantAccessor.CurrentTenantId ?? 0,
                DtFechamento = DateTime.Now,
                DtInicial = relatorio.Filtro.DataInicio ?? DateTime.Now,
                DtFinal = relatorio.Filtro.DataFim ?? DateTime.Now,
                SaldoAtual = saldoAtual,
                SaldoAnterior = saldoAnterior,
                TipoContribuicao = relatorio.Filtro.TipoContribuicao,
                UsuarioId = 1,
            };

            _context.Caixa.Add(caixa);

            await _context.SaveChangesAsync();

            return caixa;
        }

        public byte[] ExportarPdf(RelatorioMovimentoCaixaViewModel relatorioContribuicao)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 0;
            var tenant = _context.Tenant.FirstOrDefault(t => t.Id == tenantId);
            var document = new RelatorioTemploCentralMovimentoCaixa(
                relatorioContribuicao,
                tenant?.Nome ?? "Tenant",
                tenant?.Endereco ?? string.Empty,
                tenant?.PastorCongregacional ?? string.Empty);

            var pdfBytes = document.GeneratePdf();

            return pdfBytes;
        }
    }
}