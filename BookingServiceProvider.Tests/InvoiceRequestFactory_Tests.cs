using BookingServiceProvider.Entities;
using BookingServiceProvider.Factories;
using EventServiceProvider;
using Google.Protobuf.WellKnownTypes;
using UserProfileServiceProvider;

namespace BookingServiceProvider.Tests
{
    // Some AI assistance
    public class InvoiceRequestFactory_Tests
    {
        [Fact]
        public void CreateInvoiceRequest_ShouldReturnCorrectRequest()
        {
            var now = DateTime.UtcNow;
            var eventDate = Timestamp.FromDateTime(now.AddDays(10));

            // Arrange
            var factory = new InvoiceRequestFactory();
            var booking = new BookingEntity
            {
                Id = "booking123",
                UserId = "user123",
                EventId = "event123",
                TicketAmount = 2,
                Created = DateTime.UtcNow
            };
            var user = new UserProfile
            {
                FirstName = "Skurre",
                LastName = "Karlsson",
                PhoneNumber = "0707070707",
                Address = "Hökarängen",
                PostalCode = "12345",
                City = "Stockholm"
            };
            var currentEvent = new Event
            {
                EventTitle = "Nhl San Jose vs LA Kings",
                Date = eventDate,
                Price = 50
            };

            // Act
            var request = factory.CreateInvoiceRequest(booking, user, currentEvent);


            // Assert
            Assert.Equal("booking123", request.BookingId);
            Assert.Equal("Skurre", request.FirstName);
            Assert.Equal("Karlsson", request.LastName);
            Assert.Equal("0707070707", request.PhoneNumber);
            Assert.Equal("Hökarängen", request.Address);
            Assert.Equal("12345", request.PostalCode);
            Assert.Equal("Stockholm", request.City);
            Assert.Equal("Nhl San Jose vs LA Kings", request.EventName);
            Assert.Equal(currentEvent.Date.ToDateTime(), request.EventDate.ToDateTime());
            Assert.Equal(2, request.TicketAmount);
            Assert.Equal(50, request.TicketPrice);
            Assert.Equal(booking.Created.ToUniversalTime(), request.BookingDate.ToDateTime());
        }
    }
}
