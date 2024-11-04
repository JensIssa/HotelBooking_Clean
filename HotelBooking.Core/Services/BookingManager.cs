using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBooking.Core
{
    public class BookingManager : IBookingManager
    {
        private IRepository<Booking> bookingRepository;
        private IRepository<Room> roomRepository;

        // Constructor injection
        public BookingManager(IRepository<Booking> bookingRepository, IRepository<Room> roomRepository)
        {
            this.bookingRepository = bookingRepository;
            this.roomRepository = roomRepository;
        }

        public bool CreateBooking(Booking booking)
        {

            if (!IsValidBooking(booking.StartDate, booking.EndDate))
            {
                return false;
            }

            int roomId = FindAvailableRoom(booking.StartDate, booking.EndDate);

            if (roomId >= 0)
            {
                booking.RoomId = roomId;
                booking.IsActive = true;
                bookingRepository.Add(booking);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int FindAvailableRoom(DateTime startDate, DateTime endDate)
        {

            var activeBookings = bookingRepository.GetAll().Where(b => b.IsActive);
            foreach (var room in roomRepository.GetAll())
            {
                var activeBookingsForCurrentRoom = activeBookings.Where(b => b.RoomId == room.Id);
                if (activeBookingsForCurrentRoom.All(b => startDate < b.StartDate &&
                    endDate < b.StartDate || startDate > b.EndDate && endDate > b.EndDate))
                {
                    return room.Id;
                }
            }
            return -1;
        }

        public bool IsValidBooking(DateTime startDate, DateTime endDate)
        {
            // Check if start date is in the past
            if (startDate.Date < DateTime.Now.Date)
            {
                return false;
            }

            // Check if end date is before start date
            if (endDate.Date < startDate.Date)
            {
                return false;
            }

            return true;
        }

        public List<DateTime> GetFullyOccupiedDates(DateTime startDate, DateTime endDate)
        {

            List<DateTime> fullyOccupiedDates = new List<DateTime>();
            int noOfRooms = roomRepository.GetAll().Count();
            var bookings = bookingRepository.GetAll();

            if (bookings.Any())
            {
                for (DateTime d = startDate; d <= endDate; d = d.AddDays(1))
                {
                    var noOfBookings = from b in bookings
                                       where b.IsActive && d >= b.StartDate && d <= b.EndDate
                                       select b;
                    if (noOfBookings.Count() >= noOfRooms)
                        fullyOccupiedDates.Add(d);
                }
            }
            return fullyOccupiedDates;
        }

        public void RemoveBooking(int bookingId)
        {
            Booking booking = bookingRepository.Get(bookingId);
            if (booking != null)
            {
                bookingRepository.Remove(bookingId);
            }
        }

        public void EditBooking(Booking booking)
        {
            Booking modifiedBooking = bookingRepository.Get(booking.Id);
            if (modifiedBooking != null)
            {
                modifiedBooking.IsActive = booking.IsActive;
                modifiedBooking.CustomerId = booking.CustomerId;
                modifiedBooking.EndDate = booking.EndDate;
                modifiedBooking.StartDate = booking.StartDate;
                modifiedBooking.RoomId = booking.RoomId;
                bookingRepository.Edit(modifiedBooking);
            }
        }

        public Booking GetBooking(int id)
        {
            return bookingRepository.Get(id);
        }

    }
}
