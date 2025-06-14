using MediatR;
using RpslsGameService.Application.DTOs;

namespace RpslsGameService.Application.UseCases.Commands;

public record PlayGameCommand(int PlayerChoice) : IRequest<GameResultResponse>;