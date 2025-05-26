using BookingServiceProvider.Entities;
using BookingServiceProvider.Factories;
using BookingServiceProvider.Interfaces;
using EventServiceProvider;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using InvoiceServiceProvider;
using Microsoft.Extensions.Caching.Memory;
using UserProfileServiceProvider;

namespace BookingServiceProvider.Services
{
    public class BookingService(IBookingRepository bookingRepository, IMemoryCache cache, ILogger<BookingService> logger, UserProfileService.UserProfileServiceClient userClient, InvoiceServiceContract.InvoiceServiceContractClient invoiceClient, EventContract.EventContractClient eventClient, BookingReplyFactory replyFactory, InvoiceRequestFactory invoiceRequestFactory) : BookingServiceContract.BookingServiceContractBase
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IMemoryCache _cache = cache;
        private const string _cachedBookingsKey = "bookings";
        private readonly ILogger<BookingService> _logger = logger;
        private readonly BookingReplyFactory _replyFactory = replyFactory;
        private readonly InvoiceRequestFactory _invoiceRequestFactory = invoiceRequestFactory;
        private readonly UserProfileService.UserProfileServiceClient _userClient = userClient;
        private readonly InvoiceServiceContract.InvoiceServiceContractClient _invoiceClient = invoiceClient;
        private readonly EventContract.EventContractClient _eventClient = eventClient;


        // SET ALL BOOKINGS CACHE
        public async Task<IEnumerable<BookingEntity>> GetSetBookingsCacheAsync()
        {
            _cache.Remove(_cachedBookingsKey);
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            if (bookings == null)
            {
                return Enumerable.Empty<BookingEntity>();
            }

            if (!bookings.Any())
            {
                return Enumerable.Empty<BookingEntity>();
            }

            _cache.Set(_cachedBookingsKey, bookings, TimeSpan.FromHours(3));
            return bookings;
        }

        // GET ALL BOOKINGS
        public override async Task<ReplyGetAllBookings> GetAllBookings(Empty request, ServerCallContext context)
        {
            // GET ALL BOOKINGS(CACHED OR FROM DATABASE)
            if (!_cache.TryGetValue(_cachedBookingsKey, out IEnumerable<BookingEntity>? cachedBookings))
            {
                cachedBookings = await GetSetBookingsCacheAsync();
            }

            if (cachedBookings == null || !cachedBookings.Any())
            {
                _logger.LogError("Failed to retrieve bookings.");
                return _replyFactory.FailedGetAllReply("No bookings found in database.");
            }

            // GET USERS AND PUT THEM IN DICTIONARY TO AVOID MULTIPLE CALLS IN FORLOOP
            var users = await _userClient.GetAllUserProfilesAsync(new Empty());
            var userDict = users.AllUserProfiles.ToDictionary(u => u.UserId);

            // GET EVENTS AND PUT THEM IN DICTIONARY TO AVOID MULTIPLE CALLS IN FORLOOP
            var events = _eventClient.GetEvents(new Empty());
            var eventDict = events.Events.ToDictionary(e => e.EventId);

            var bookingsReply = new ReplyGetAllBookings { IsSuccess = true, Message = "Bookings retrieved successfully." };

            foreach (var booking in cachedBookings)
            {
                var user = userDict[booking.UserId];
                var currentEvent = eventDict[booking.EventId];

                var bookingReply = _replyFactory.SuccessGetAllReply( booking, user, currentEvent );
                bookingsReply.Bookings.Add(bookingReply);
            }
            return bookingsReply;
        }

        // GET ALL BOOKINGS BY USER ID
        public override async Task<ReplyGetAllBookings> GetAllBookingsByUserId(RequestGetAllBookingsByUserId request, ServerCallContext context)
        {

            if (string.IsNullOrEmpty(request.Userid))
            {
                return _replyFactory.FailedGetAllReply("request.UserId is null or empty.");
            }

            // GET ALL BOOKINGS(CACHED OR FROM DATABASE)
            if (!_cache.TryGetValue(_cachedBookingsKey, out IEnumerable<BookingEntity>? cachedBookings))
            {
                cachedBookings = await GetSetBookingsCacheAsync();
            }

            if (cachedBookings == null || !cachedBookings.Any())
            {
                _logger.LogError("Failed to retrieve bookings.");
                return _replyFactory.FailedGetAllReply("No bookings found in database.");
            }

            var userBookings = cachedBookings.Where(booking => booking.UserId == request.Userid).ToList();

            if (!userBookings.Any())
            {
                _logger.LogError($"No bookings found for user with UserId: {request.Userid}.");
                return _replyFactory.FailedGetAllReply($"No bookings found for user with UserId: {request.Userid}.");
            }

            // GET USERS AND PUT THEM IN DICTIONARY TO AVOID MULTIPLE CALLS IN FORLOOP
            var users = await _userClient.GetAllUserProfilesAsync(new Empty());
            var userDict = users.AllUserProfiles.ToDictionary(u => u.UserId);

            // GET EVENTS AND PUT THEM IN DICTIONARY TO AVOID MULTIPLE CALLS IN FORLOOP
            var events = _eventClient.GetEvents(new Empty());
            var eventDict = events.Events.ToDictionary(e => e.EventId);

            var bookingsReply = new ReplyGetAllBookings { IsSuccess = true, Message = "User bookings retrieved successfully." };

            foreach (var booking in userBookings)
            {
                var user = userDict[booking.UserId];
                var currentEvent = eventDict[booking.EventId];

                var bookingReply = _replyFactory.SuccessGetAllReply(booking, user, currentEvent);
                bookingsReply.Bookings.Add(bookingReply);
            }
            return bookingsReply;
        }

        // GET BOOKING
        public override async Task<ReplyGetBooking> GetBooking(RequestGetBooking request, ServerCallContext context)
        {
            // VALIDATE REQUEST (Booking Id)
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                _logger.LogError("request.Id is null or empty.");
                return _replyFactory.FailedGetReply("request.Id is null or empty.");
            }

            // GET SPECIFIC BOOKING
            var booking = await _bookingRepository.GetBookingByIdAsync(request.Id);
            if (booking == null)
            {
                _logger.LogError("Booking was not found.");
                return _replyFactory.FailedGetReply($"Booking was not found at, (Id: {request.Id})");
            }
            _logger.LogInformation($"Booking retrieved successfully at, (Id: {booking.Id}).");

            // GET USER
            var user = await _userClient.GetUserProfileByIdAsync(new RequestByUserId { UserId = booking.UserId });
            if (user == null)
            {
                _logger.LogError("User was not found.");
                return _replyFactory.FailedGetReply($"User needed for booking reply was not found at, (UserId: {booking.UserId}) ");
            }
            _logger.LogInformation($"User profile retrieved successfully at, (UserId: {user.Profile.UserId}).");

            // GET EVENT
            var currentEvent = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = booking!.EventId });
            if (currentEvent == null)
            {
                _logger.LogError("Event was not found.");
                return _replyFactory.FailedGetReply("Event needed for booking reply was not found.");
            }
            _logger.LogInformation($"Event retrieved successfully at, (EventId: {currentEvent.Event.EventId}).");

            var bookingReply = _replyFactory.SuccessGetReply(booking, user.Profile, currentEvent.Event, "Booking retrieved successfully.");
            return bookingReply;
        }

        // CREATE BOOKING
        public override async Task<ReplyCreateBooking> CreateBooking(RequestCreateBooking request, ServerCallContext context)
        {
            // VALIDATE REQUEST (UserId, EventId, TicketAmount)
            var validationResult = ValidateCreateRequest(request);

            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            // INSTANTIATE BOOKING ENTITY 
            var newBooking = new BookingEntity
            {
                UserId = request.Userid,
                EventId = request.Eventid,
                TicketAmount = request.Ticketamount,
            };

            // CREATE NEW BOOKING IN DATABASE
            var bookingResponse = await _bookingRepository.CreateBookingAsync(newBooking);
            if (!bookingResponse)
            {
                _logger.LogError("Failed to create booking.");
                return _replyFactory.FailedCreateReply("Failed to create booking.");
            }
            _logger.LogInformation("Booking created successfully.");

            //GET USER
            var userProfile = await _userClient.GetUserProfileByIdAsync(new RequestByUserId { UserId = newBooking.UserId });
            if (userProfile == null)
            {
                _logger.LogError($"Failed to retrieve user profile at, (UserId: {newBooking.UserId}).");
                return _replyFactory.FailedCreateReply("Failed to retrieve specified user profile needed to create booking.");
            }
            _logger.LogInformation($"User profile retrieved successfully at, (UserId: {userProfile.Profile.UserId}).");

            // GET EVENT
            var currentEvent = await _eventClient.GetEventByIdAsync(new GetEventByIdRequest { EventId = newBooking.EventId });
            if (currentEvent == null)
            {
                _logger.LogError($"Failed to retrieve event at, (EventId: {newBooking.EventId}).");
                return _replyFactory.FailedCreateReply("Failed to retrieve specified event needed to create booking.");
            }
            _logger.LogInformation($"Event retrieved successfully at, (EventId: {currentEvent.Event.EventId}).");

            // MAP INVOICE REQUEST FOR INVOICE CREATION
            var invoiceRequest = _invoiceRequestFactory.CreateInvoiceRequest(newBooking, userProfile.Profile, currentEvent.Event);

            // CREATE INVOICE
            var invoiceResponse = await _invoiceClient.CreateInvoiceAsync(invoiceRequest);

            if (!invoiceResponse.Succeeded)
            {
                // DELETE BOOKING ROLLBACK IF INVOICE CREATION WAS NOT SUCCESSFUL
                await _bookingRepository.DeleteBookingAsync(newBooking.Id);
                _logger.LogError($"Failed to create invoice. Booking was created, but later deleted.");

                return _replyFactory.FailedCreateReply("Failed to create invoice. Booking was created, but later deleted.");
            }

            // CLEAR CACHE TO AVOID STALE CACHE
            _cache.Remove(_cachedBookingsKey);

            _logger.LogInformation("Booking and invoice created successfully.");
            return _replyFactory.SuccessCreateReply("Booking and invoice created successfully.");
        }

        // DELETE BOOKING
        public override async Task<ReplyDeleteBooking> DeleteBooking(RequestDeleteBooking request, ServerCallContext context)
        {
            // GET SPECIFIC BOOKING FOR DELETION
            var booking = await _bookingRepository.GetBookingByIdAsync(request.Id);
            if (booking == null)
            {
                _logger.LogError($"Failed to find booking at, (Id: {request.Id}), no booking was deleted.");
                return _replyFactory.FailedDeleteReply($"Failed to find booking at, (Id: {request.Id}), no booking was deleted.");
            }

            // DELETE BOOKING
            var isBookingDeleted = await _bookingRepository.DeleteBookingAsync(booking.Id);
            if (!isBookingDeleted)
            {
                _logger.LogError($"Falied to delete booking at, (Id: {booking.Id}).");
                return _replyFactory.FailedDeleteReply($"Failed to delete booking at (Id: {booking.Id}).");
            }

            // CLEAR CACHE TO AVOID STALE CACHE
            _cache.Remove(_cachedBookingsKey);
            _logger.LogInformation($"Booking (ID: {booking.Id}) was deleted successfully.");
            return _replyFactory.SuccessDeleteReply($"Booking at, (ID: {booking.Id}) was deleted successfully.");
        }

        // VALIDATE INCOMING CREATE BOOKING REQUEST
        private ReplyCreateBooking ValidateCreateRequest(RequestCreateBooking request)
        {
            if (string.IsNullOrWhiteSpace(request.Userid))
            {
                _logger.LogError("request.UserId is null or empty.");
                return new ReplyCreateBooking { IsSuccess = false, Message = "request.UserId is null or empty." };
            }

            if (string.IsNullOrWhiteSpace(request.Eventid))
            {
                _logger.LogError("request.EventId is null or empty.");
                return new ReplyCreateBooking { IsSuccess = false, Message = "request.EventId is null or empty." };
            }

            if (request.Ticketamount == 0)
            {
                _logger.LogError("Invalid ticket amount, must be atleast 1.");
                return new ReplyCreateBooking { IsSuccess = false, Message = "Invalid ticket amount, must be atleast 1." };
            }

            return new ReplyCreateBooking { IsSuccess = true };
        }
    }

}

