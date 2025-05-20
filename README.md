
# BookingServiceProvider

A **gRPC microservice** built with **ASP.NET Core**, responsible for managing bookings. Part of a larger **Event system** composed of
multiple microservices.
This microservice communicates with other microservices (such as `UserProfileServiceProvider` and `InvoiceServiceProvider`).

## How to use

As a client: Copy the proto-file (***booking.proto***) from BookingServiceProvider/Protos to your project.

### Install Packages

- Grpc.Tools
- Grpc.Net.Client
- Grpc.Net.ClientFactory
- Google.Protobuf

### Example use (Mostly AI generated)

```csharp
// gRPC client setup
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var bookingClient = new BookingServiceContract.BookingServiceContractClient(channel);

// Get all bookings
var getAllReply = await bookingClient.GetAllBookingsAsync(new Empty());

if (getAllReply.IsSuccess)
{
    foreach (var booking in getAllReply.Bookings)
    {
        Console.WriteLine($"Booking ID: {booking.BookingId}, User: {booking.User.Firstname} {booking.User.Lastname}, Event: {booking.Event.Title}");
    }
}
else
{
    Console.WriteLine($"Error retrieving bookings: {getAllReply.Message}");
}

// Get booking
var getRequest = new RequestGetBooking
{
    Id = "booking-789"
};

var getReply = await bookingClient.GetBookingAsync(getRequest);

if (getReply.IsSuccess)
{
    Console.WriteLine($"Booking found: User {getReply.User.Firstname} {getReply.User.Lastname}, Event {getReply.Event.Title}");
}
else
{
    Console.WriteLine($"Failed to retrieve booking: {getReply.Message}");
}

// Example: Create a new booking
var createRequest = new RequestCreateBooking
{
    Userid = "user-123",
    Eventid = "event-456",
    Ticketamount = 2
};

var createReply = await bookingClient.CreateBookingAsync(createRequest);

if (createReply.IsSuccess)
{
    Console.WriteLine("Booking and invoice created successfully.");
}
else
{
    Console.WriteLine($"Failed to create booking: {createReply.Message}");
}

// Delete booking
var deleteRequest = new RequestDeleteBooking
{
    Id = "booking-789"
};

var deleteReply = await bookingClient.DeleteBookingAsync(deleteRequest);

if (deleteReply.IsSuccess)
{
    Console.WriteLine("Booking deleted successfully.");
}
else
{
    Console.WriteLine($"Failed to delete booking: {deleteReply.Message}");
}
```
