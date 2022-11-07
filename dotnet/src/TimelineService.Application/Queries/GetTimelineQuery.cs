using MediatR;
using TimelineService.Domain.ViewModels;

namespace TimelineService.Application.Queries
{
    public record GetTimelineQuery(string username) : IRequest<TimelineViewModel>;
}
