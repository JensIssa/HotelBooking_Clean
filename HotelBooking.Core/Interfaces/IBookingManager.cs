﻿using System;
using System.Collections.Generic;

namespace HotelBooking.Core
{
    public interface IBookingManager
    {
        bool CreateBooking(Booking booking);
        int FindAvailableRoom(DateTime startDate, DateTime endDate);
        List<DateTime> GetFullyOccupiedDates(DateTime startDate, DateTime endDate);
        void RemoveBooking(int bookingId);

        void EditBooking(Booking booking);
        public Booking GetBooking(int id);

    }
}
