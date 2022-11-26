namespace TimelineService.Domain.Entities
{
    public class HonkEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
