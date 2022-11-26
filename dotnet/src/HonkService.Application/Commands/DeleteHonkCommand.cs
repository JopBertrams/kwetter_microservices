using HonkService.Application.Common;
using MediatR;

namespace HonkService.Application.Commands
{
    public record DeleteHonkCommand(Guid honkId) : IRequest<DeletedHonkResult>;
}
