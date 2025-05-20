namespace BookingServiceProvider.Entities
{
    public class BookingEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string EventId { get; set; } = null!;
        public int TicketAmount { get; set; } = 1;
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
