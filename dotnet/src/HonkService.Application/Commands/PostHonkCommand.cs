using HonkService.Application.Common;
using MediatR;

namespace HonkService.Application.Commands
{
    public record PostHonkCommand(string Username, string Message) : IRequest<HonkResult>;
}
