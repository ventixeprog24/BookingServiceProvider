using BookingServiceProvider.Contexts;
using BookingServiceProvider.Entities;
using BookingServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingServiceProvider.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<BookingEntity> _bookingsTable;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(DataContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _bookingsTable = context.Set<BookingEntity>();
            _logger = logger;
        }

        // GET BY ID
        public async Task<BookingEntity?> GetBookingByIdAsync(string id)
        {
            try
            {
                var booking = await _bookingsTable.FirstOrDefaultAsync(b => b.Id == id);

                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving booking.");
                return null;
            }
        }

        // GET ALL
        public async Task<IEnumerable<BookingEntity>> GetAllBookingsAsync()
        {
            try
            {
                return await _bookingsTable.ToListAsync();
            }
            catch
            {
                _logger.LogError("Error while retrieving all bookings.");
                return Enumerable.Empty<BookingEntity>();
            }
        }

        // CREATE
        public async Task<bool> CreateBookingAsync(BookingEntity booking)
        {
            try
            {
                if (booking != null)
                {
                    await _bookingsTable.AddAsync(booking);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    _logger.LogWarning("Booking could not be created.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating booking.");
                return false;
            }
        }

        // UPDATE
        public async Task<bool> UpdateBookingAsync(BookingEntity booking)
        {
            try
            {
                if (booking != null)
                {
                    _bookingsTable.Update(booking);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    _logger.LogWarning("Booking could not be found for update.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating booking.");
                return false;
            }
        }

        // DELETE
        public async Task<bool> DeleteBookingAsync(string id)
        {
            try
            {
                var booking = await GetBookingByIdAsync(id);
                if (booking != null)
                {
                    _bookingsTable.Remove(booking);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    _logger.LogWarning("Booking could not be found for deletion.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting booking.");
                return false;
            }
        }
    }
}
