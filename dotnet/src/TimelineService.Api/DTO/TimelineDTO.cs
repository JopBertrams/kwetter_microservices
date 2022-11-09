using TimelineService.Domain.Entities;

namespace TimelineService.Api.DTO
{
    public record TimelineDTO(
        string Username,
        List<HonkEntity> Honks
    );
}
