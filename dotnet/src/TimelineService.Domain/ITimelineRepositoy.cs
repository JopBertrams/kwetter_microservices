using TimelineService.Domain.Entities;

namespace TimelineService.Domain
{
    public interface ITimelineRepositoy
    {
        void CreateTimeline(HonkEntity timeline);
        
        IEnumerable<HonkEntity?>? GetTimeline(string id);
    }
}
