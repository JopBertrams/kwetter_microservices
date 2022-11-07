using System.Text.Json;
using StackExchange.Redis;
using TimelineService.Domain;
using TimelineService.Domain.Entities;

namespace TimelineService.Application
{
    public class TimelineRepository : ITimelineRepositoy
    {
        private readonly IConnectionMultiplexer _redis;

        public TimelineRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public void CreateTimeline(HonkEntity honk)
        {
            if (honk == null)
            {
                throw new ArgumentOutOfRangeException(nameof(honk));
            }

            var db = _redis.GetDatabase();

            var serialMessage = JsonSerializer.Serialize(honk);

            db.SortedSetAdd(honk.Username, serialMessage, ((DateTimeOffset)honk.CreatedAt).ToUnixTimeSeconds());
        }

        public IEnumerable<HonkEntity?>? GetTimeline(string username)
        {
            var db = _redis.GetDatabase();

            var timeline = db.SortedSetRangeByRank(username);

            return timeline.Select(x => JsonSerializer.Deserialize<HonkEntity>(x)).ToList();
        }
    }
}
