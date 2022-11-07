using MediatR;
using TimelineService.Domain;
using TimelineService.Domain.ViewModels;

namespace TimelineService.Application.Queries
{
    public class GetTimelineQueryHandler : IRequestHandler<GetTimelineQuery, TimelineViewModel>
    {
        private readonly ITimelineRepositoy _timelineRepository;

        public GetTimelineQueryHandler(ITimelineRepositoy timelineRepository)
        {
            _timelineRepository = timelineRepository ?? throw new ArgumentNullException(nameof(timelineRepository));
        }

        public async Task<TimelineViewModel> Handle(GetTimelineQuery query, CancellationToken cancellationToken)
        {
            var honkEntities = _timelineRepository.GetTimeline(query.username);

            return new TimelineViewModel(query.username, honkEntities);
        }
    }
}
