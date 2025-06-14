using MediatR;
using RpslsGameService.Application.DTOs;

namespace RpslsGameService.Application.UseCases.Queries;

public record GetChoicesQuery : IRequest<IReadOnlyList<ChoiceDto>>;