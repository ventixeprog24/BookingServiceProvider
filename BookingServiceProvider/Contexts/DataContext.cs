using BookingServiceProvider.Entities;
using Microsoft.EntityFrameworkCore;


namespace BookingServiceProvider.Contexts
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<BookingEntity> Bookings { get; set; } = null!;
    }
}
