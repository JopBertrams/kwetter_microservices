namespace HonkService.Application.Common
{
    public record HonkResult(
        Guid Id,
        string UserId,
        string Message,
        DateTime CreatedAt
    );
}
