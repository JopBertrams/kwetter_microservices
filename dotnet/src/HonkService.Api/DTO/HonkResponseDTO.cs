namespace HonkService.Api.DTO
{
    public record HonkResponseDTO(
        Guid Id,
        string UserId,
        string Message,
        DateTime CreatedAt
    );
}