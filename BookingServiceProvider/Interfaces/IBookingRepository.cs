using BookingServiceProvider.Entities;

namespace BookingServiceProvider.Interfaces
{
    public interface IBookingRepository
    {
        Task<BookingEntity?> GetBookingByIdAsync(string id);
        Task<IEnumerable<BookingEntity>> GetAllBookingsAsync();
        Task<bool> CreateBookingAsync(BookingEntity booking);
        Task<bool> UpdateBookingAsync(BookingEntity booking);
        Task<bool> DeleteBookingAsync(string id);
    }
}
