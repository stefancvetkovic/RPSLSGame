using MediatR;
using RpslsGameService.Application.DTOs;

namespace RpslsGameService.Application.UseCases.Queries;

public record GetGameHistoryQuery : IRequest<GameHistoryResponse>;