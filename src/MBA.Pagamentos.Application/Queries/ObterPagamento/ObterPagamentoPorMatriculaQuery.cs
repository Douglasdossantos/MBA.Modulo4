using MBA.Pagamentos.Application.Queries.Dtos;
using MediatR;

namespace MBA.Pagamentos.Application.Queries.ObterPagamento;

public record ObterPagamentoPorMatriculaQuery(Guid MatriculaId) : IRequest<PagamentoStatusDto?>;
