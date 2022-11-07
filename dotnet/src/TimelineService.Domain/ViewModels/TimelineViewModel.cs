using TimelineService.Domain.Entities;

namespace TimelineService.Domain.ViewModels
{
    public record TimelineViewModel(
        string Username,
        IEnumerable<HonkEntity> Honks
    );
}
