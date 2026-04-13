using MBA.Pagamentos.Application.Queries.Dtos;
using MBA.Pagamentos.Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Application.Queries.ObterPagamento;

public class ObterPagamentoQueryHandler(FaturamentoDbContext context) :
    IRequestHandler<ObterPagamentoPorMatriculaQuery, PagamentoStatusDto?>,
    IRequestHandler<ObterPagamentoPorIdQuery, PagamentoStatusDto?>
{
    private readonly FaturamentoDbContext _context = context;

    public async Task<PagamentoStatusDto?> Handle(ObterPagamentoPorMatriculaQuery request, CancellationToken cancellationToken)
    {
        return await _context.Pagamentos
            .AsNoTracking()
            .Where(p => p.MatriculaId == request.MatriculaId)
            .Select(p => new PagamentoStatusDto
            {
                Id = p.Id,
                MatriculaCursoId = p.MatriculaId,
                Valor = p.Valor,
                Status = p.StatusPagamento.Status.ToString(),
                DataPagamento = p.DataPagamento,
                TransacaoId = p.CodigoConfirmacaoPagamento
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagamentoStatusDto?> Handle(ObterPagamentoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Pagamentos
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new PagamentoStatusDto
            {
                Id = p.Id,
                MatriculaCursoId = p.MatriculaId,
                Valor = p.Valor,
                Status = p.StatusPagamento.Status.ToString(),
                DataPagamento = p.DataPagamento,
                TransacaoId = p.CodigoConfirmacaoPagamento
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
