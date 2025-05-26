using BookingServiceProvider.Entities;
using EventServiceProvider;
using Google.Protobuf.WellKnownTypes;
using InvoiceServiceProvider;
using UserProfileServiceProvider;

namespace BookingServiceProvider.Factories
{
    public class InvoiceRequestFactory
    {
        public RequestCreateInvoice CreateInvoiceRequest(BookingEntity booking, UserProfile user, Event currentEvent)
        {
            return new RequestCreateInvoice
            {
                BookingId = booking.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Email = user.Email,
                PostalCode = user.PostalCode,
                City = user.City,
                EventName = currentEvent.EventTitle,
                EventDate = currentEvent.Date,
                TicketAmount = booking.TicketAmount,
                TicketPrice = currentEvent.Price,
                BookingDate = Timestamp.FromDateTime(booking.Created.ToUniversalTime())
            };
        }
    }
}
