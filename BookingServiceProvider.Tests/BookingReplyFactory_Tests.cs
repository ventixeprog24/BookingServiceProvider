using BookingServiceProvider.Entities;
using BookingServiceProvider.Factories;
using EventServiceProvider;
using Google.Protobuf.WellKnownTypes;
using UserProfileServiceProvider;

namespace BookingServiceProvider.Tests
{
    // Some AI assistance
    public class BookingReplyFactory_Tests
    {
        [Fact]
        public void SuccessCreateReply_ShouldReturnSuccessReply()
        {
            // Arrange
            var factory = new BookingReplyFactory();
            string message = "Booking created successfully.";

            // Act
            var reply = factory.SuccessCreateReply(message);

            // Assert
            Assert.True(reply.IsSuccess);
            Assert.Equal(message, reply.Message);
        }

        [Fact]
        public void FailedCreateReply_ShouldReturnFailedReply()
        {
            // Arrange
            var factory = new BookingReplyFactory();
            string message = "Booking creation failed.";

            // Act
            var reply = factory.FailedCreateReply(message);

            // Assert
            Assert.False(reply.IsSuccess);
            Assert.Equal(message, reply.Message);
        }

        [Fact]
        public void SuccessDeleteReply_ShouldReturnSuccessReply()
        {
            // Arrange
            var factory = new BookingReplyFactory();
            string message = "Booking deleted successfully.";

            // Act
            var reply = factory.SuccessDeleteReply(message);

            // Assert
            Assert.True(reply.IsSuccess);
            Assert.Equal(message, reply.Message);
        }

        [Fact]
        public void FailedDeleteReply_ShouldReturnFailedReply()
        {
            // Arrange
            var factory = new BookingReplyFactory();
            string message = "Booking deletion failed.";

            // Act
            var reply = factory.FailedDeleteReply(message);

            // Assert
            Assert.False(reply.IsSuccess);
            Assert.Equal(message, reply.Message);
        }

        [Fact]
        public void SuccessGetReply_ShouldReturnCorrectReply()
        {
            // Arrange
            var factory = new BookingReplyFactory();

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
                Email = "skurban@live.com",
                PhoneNumber = "0707070707",
                Address = "Hökarängen",
                PostalCode = "12345",
                City = "Stockholm"
            };

            var eventDate = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(5));
            var currentEvent = new Event
            {
                EventTitle = "Nhl San Jose vs LA Kings",
                Price = 100,
                Date = eventDate
            };

            string message = "Booking retrieved successfully.";

            // Act
            var reply = factory.SuccessGetReply(booking, user, currentEvent, message);

            // Assert
            Assert.True(reply.IsSuccess);
            Assert.Equal(message, reply.Message);

            var bookingReply = reply.Booking;
            Assert.Equal(booking.Id, bookingReply.Bookingid);
            Assert.Equal(booking.UserId, bookingReply.Userid);
            Assert.Equal(booking.EventId, bookingReply.Eventid);
            Assert.Equal(user.FirstName, bookingReply.Firstname);
            Assert.Equal(user.LastName, bookingReply.Lastname);
            Assert.Equal(user.Email, bookingReply.Email);
            Assert.Equal(user.PhoneNumber, bookingReply.Phone);
            Assert.Equal(user.Address, bookingReply.Address);
            Assert.Equal(user.PostalCode, bookingReply.Postalcode);
            Assert.Equal(user.City, bookingReply.City);
            Assert.Equal(currentEvent.EventTitle, bookingReply.Eventname);
            Assert.Equal(booking.TicketAmount, bookingReply.Ticketamount);
            Assert.Equal(currentEvent.Price, bookingReply.Ticketprice);
            Assert.Equal(currentEvent.Price * booking.TicketAmount, bookingReply.Totalprice);
            Assert.Equal(currentEvent.Date, bookingReply.Eventdate);
            Assert.Equal(Timestamp.FromDateTime(booking.Created.ToUniversalTime()), bookingReply.Created);
        }

        [Fact]
        public void FailedGetReply_ShouldReturnFailedReply()
        {
            // Arrange
            var factory = new BookingReplyFactory();
            string message = "Failed to retrieve booking.";

            // Act
            var reply = factory.FailedGetReply(message);

            // Assert
            Assert.False(reply.IsSuccess);
            Assert.Equal(message, reply.Message);
        }

        [Fact]
        public void SuccessGetAllReply_ShouldReturnCorrectBooking()
        {
            // Arrange
            var factory = new BookingReplyFactory();

            var booking = new BookingEntity
            {
                Id = "booking123",
                UserId = "user123",
                EventId = "event123",
                TicketAmount = 3,
                Created = DateTime.UtcNow
            };

            var user = new UserProfile
            {
                FirstName = "Skurre",
                LastName = "Karlsson",
                Email = "skurban@live.com",
                PhoneNumber = "0707070707",
                Address = "Hökarängen",
                PostalCode = "54321",
                City = "Stockholm"
            };

            var eventDate = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(7));
            var currentEvent = new Event
            {
                EventTitle = "Nhl San Jose vs LA Kings",
                Price = 200,
                Date = eventDate
            };

            // Act
            var result = factory.SuccessGetAllReply(booking, user, currentEvent);

            // Assert
            Assert.Equal(booking.Id, result.Bookingid);
            Assert.Equal(booking.UserId, result.Userid);
            Assert.Equal(booking.EventId, result.Eventid);
            Assert.Equal(user.FirstName, result.Firstname);
            Assert.Equal(user.LastName, result.Lastname);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PhoneNumber, result.Phone);
            Assert.Equal(user.Address, result.Address);
            Assert.Equal(user.PostalCode, result.Postalcode);
            Assert.Equal(user.City, result.City);
            Assert.Equal(currentEvent.EventTitle, result.Eventname);
            Assert.Equal(booking.TicketAmount, result.Ticketamount);
            Assert.Equal(currentEvent.Price, result.Ticketprice);
            Assert.Equal(currentEvent.Price * booking.TicketAmount, result.Totalprice);
            Assert.Equal(currentEvent.Date, result.Eventdate);
            Assert.Equal(Timestamp.FromDateTime(booking.Created.ToUniversalTime()), result.Created);
        }

        [Fact]
        public void FailedGetAllReply_ShouldReturnFailedReplyWithMessage()
        {
            // Arrange
            var factory = new BookingReplyFactory(); 
            var expectedMessage = "Failed to retrieve all bookings.";

            // Act
            var result = factory.FailedGetAllReply(expectedMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
    
