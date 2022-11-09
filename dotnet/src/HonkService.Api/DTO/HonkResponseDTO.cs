namespace HonkService.Api.DTO
{
    public record HonkResponseDTO(
        Guid Id,
        string Username,
        string Message,
        DateTime CreatedAt
    );
}