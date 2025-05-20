using BookingServiceProvider.Entities;
using EventServiceProvider;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
using UserProfileServiceProvider;

namespace BookingServiceProvider.Factories
{
    public class BookingReplyFactory
    {
        // SUCCESS CREATE BOOKING REPLY
        public ReplyCreateBooking SuccessCreateReply(string message)
        {
            return new ReplyCreateBooking { IsSuccess = true, Message = message };
        }

        // FAILED CREATE BOOKING REPLY
        public ReplyCreateBooking FailedCreateReply(string message)
        {
            return new ReplyCreateBooking { IsSuccess = false, Message = message };
        }

        // SUCCESS DELETE BOOKING REPLY
        public ReplyDeleteBooking SuccessDeleteReply(string message)
        {
            return new ReplyDeleteBooking { IsSuccess = true, Message = message };
        }

        // FAILED DELETE BOOKING REPLY
        public ReplyDeleteBooking FailedDeleteReply(string message)
        {
            return new ReplyDeleteBooking { IsSuccess = false, Message = message };
        }

        // SUCCESS GET BOOKING REPLY
        public ReplyGetBooking SuccessGetReply(BookingEntity booking, UserProfile userProfile, Event currentEvent, string message)
        {
            return new ReplyGetBooking
            {
                IsSuccess = true,
                Message = message,
                Booking = new Booking
                {
                    Bookingid = booking!.Id,
                    Userid = booking.UserId,
                    Eventid = booking.EventId,
                    Firstname = userProfile.FirstName,
                    Lastname = userProfile.LastName,
                    Email = userProfile.Email,
                    Phone = userProfile.PhoneNumber,
                    Address = userProfile.Address,
                    Postalcode = userProfile.PostalCode,
                    City = userProfile.City,
                    Eventname = currentEvent.EventTitle,
                    Ticketamount = booking.TicketAmount,
                    Ticketprice = currentEvent.Price,
                    Totalprice = currentEvent.Price * booking.TicketAmount,
                    Eventdate = currentEvent.Date,
                    Created = Timestamp.FromDateTime(booking.Created.ToUniversalTime()),
                }
            };
        }

        // FAILED GET BOOKING REPLY
        public ReplyGetBooking FailedGetReply(string message)
        {
            return new ReplyGetBooking { IsSuccess = false, Message = message };
        }

        // SUCCESS GET ALL BOOKINGS REPLY
        public Booking SuccessGetAllReply(BookingEntity booking, UserProfile userProfile, Event currentEvent)
        {
            var bookingReply = new Booking
            {
                Bookingid = booking!.Id,
                Userid = booking.UserId,
                Eventid = booking.EventId,
                Firstname = userProfile.FirstName,
                Lastname = userProfile.LastName,
                Email = userProfile.Email,
                Phone = userProfile.PhoneNumber,
                Address = userProfile.Address,
                Postalcode = userProfile.PostalCode,
                City = userProfile.City,
                Eventname = currentEvent.EventTitle,
                Ticketamount = booking.TicketAmount,
                Ticketprice = currentEvent.Price,
                Totalprice = currentEvent.Price * booking.TicketAmount,
                Eventdate = currentEvent.Date,
                Created = Timestamp.FromDateTime(booking.Created.ToUniversalTime())
            };

            return bookingReply;
        }

        // FAILED GET ALL BOOKINGS REPLY
        public ReplyGetAllBookings FailedGetAllReply(string message)
        {
            return new ReplyGetAllBookings { IsSuccess = false, Message = message };
        }
    }
}
