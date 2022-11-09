namespace HonkService.Application.Common
{
    public record HonkResult(
        Guid Id,
        string Username,
        string Message,
        DateTime CreatedAt
    );
}
