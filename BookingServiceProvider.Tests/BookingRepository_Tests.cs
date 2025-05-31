using BookingServiceProvider.Contexts;
using BookingServiceProvider.Entities;
using BookingServiceProvider.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookingServiceProvider.Tests
{
    public class BookingRepository_Tests
    {
        // Mycket AI hjälp
        private static DbContextOptions<DataContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldReturnTrue_WhenBookingIsValid()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            var booking = new BookingEntity
            {
                UserId = "user-123",
                EventId = "event-456",
                TicketAmount = 2
            };

            // Act
            var result = await repository.CreateBookingAsync(booking);

            // Assert
            Assert.True(result);
            Assert.Single(context.Bookings);
        }

        [Fact]
        public async Task GetBookingByIdAsync_ShouldReturnBooking_WhenIdIsValid()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            var booking = new BookingEntity
            {
                Id = "booking-123",
                UserId = "user-1",
                EventId = "event-2",
                TicketAmount = 2
            };

            await context.Bookings.AddAsync(booking);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetBookingByIdAsync("booking-123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user-1", result!.UserId);
        }

        [Fact]
        public async Task GetBookingByIdAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            // Act
            var result = await repository.GetBookingByIdAsync("non-existent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllBookingsAsync_ShouldReturnAllBookings()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            var bookings = new List<BookingEntity>
            {
                new() { UserId = "user-1", EventId = "event-1", TicketAmount = 1 },
                new() { UserId = "user-2", EventId = "event-2", TicketAmount = 2 }
            };

            await context.Bookings.AddRangeAsync(bookings);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllBookingsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateBookingAsync_ShouldUpdateBooking_WhenBookingExists()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            var booking = new BookingEntity
            {
                Id = "booking-123",
                UserId = "user-1",
                EventId = "event-1",
                TicketAmount = 1
            };

            await context.Bookings.AddAsync(booking);
            await context.SaveChangesAsync();

            // Act
            booking.TicketAmount = 5;
            var result = await repository.UpdateBookingAsync(booking);

            // Assert
            Assert.True(result);
            var updated = await context.Bookings.FindAsync("booking-123");
            Assert.Equal(5, updated!.TicketAmount);
        }

        [Fact]
        public async Task UpdateBookingAsync_ShouldReturnFalse_WhenBookingIsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            // Act
            var result = await repository.UpdateBookingAsync(null!);

            // Assert
            Assert.False(result);
            logger.Received(1).LogWarning("Booking could not be found for update.");
        }

        [Fact]
        public async Task DeleteBookingAsync_ShouldRemoveBooking_WhenIdIsValid()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            var booking = new BookingEntity
            {
                Id = "booking-123",
                UserId = "user-1",
                EventId = "event-1",
                TicketAmount = 2
            };

            await context.Bookings.AddAsync(booking);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteBookingAsync("booking-123");

            // Assert
            Assert.True(result);
            var deleted = await context.Bookings.FindAsync("booking-123");
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteBookingAsync_ShouldReturnFalse_WhenBookingDoesNotExist()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new DataContext(options);
            var logger = Substitute.For<ILogger<BookingRepository>>();
            var repository = new BookingRepository(context, logger);

            // Act
            var result = await repository.DeleteBookingAsync("non-existent-id");

            // Assert
            Assert.False(result);
            logger.Received(1).LogWarning("Booking could not be found for deletion.");
        }

    }
}


