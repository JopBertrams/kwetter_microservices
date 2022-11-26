using MediatR;
using TimelineService.Domain.Entities;

namespace TimelineService.Application.Commands
{
    public record CreateTimelineCommand(HonkEntity HonkEntity) : IRequest<HonkEntity>;
}
