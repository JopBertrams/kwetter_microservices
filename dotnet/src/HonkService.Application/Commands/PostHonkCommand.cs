using HonkService.Application.Common;
using MediatR;

namespace HonkService.Application.Commands
{
    public record PostHonkCommand(string UserId, string Message) : IRequest<HonkResult>;
}
