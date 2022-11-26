using MediatR;
using TimelineService.Domain;
using TimelineService.Domain.Entities;

namespace TimelineService.Application.Commands
{
    public class CreateTimelineCommandHandler : IRequestHandler<CreateTimelineCommand, HonkEntity>
    {
        private readonly ITimelineRepositoy _timelineRepository;

        public CreateTimelineCommandHandler(ITimelineRepositoy timelineRepository)
        {
            _timelineRepository = timelineRepository ?? throw new ArgumentNullException(nameof(timelineRepository));
        }

        public Task<HonkEntity> Handle(CreateTimelineCommand command, CancellationToken cancellationToken)
        {
            _timelineRepository.CreateTimeline(command.HonkEntity);

            return Task.FromResult(command.HonkEntity);
        }
    }
}
