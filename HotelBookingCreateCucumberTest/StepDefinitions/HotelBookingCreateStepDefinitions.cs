using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using HotelBooking.Core;
using HotelBooking.Infrastructure.Repositories;

namespace HotelBookingCreateCucumberTest.StepDefinitions
{
    [Binding]
    public sealed class HotelBookingCreateStepDefinitions
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _isRoomAvailable;

        private DateTime _bookingStartDate;
        private DateTime _bookingEndDate;
        private bool _bookingResult;

        private readonly Mock<IRepository<Booking>> _mockBookingRepository;
        private readonly Mock<IRepository<Room>> _mockRoomRepository;
        private readonly BookingManager _bookingManager;
        private ArgumentException _expectedException;


        public HotelBookingCreateStepDefinitions()
        {
            _mockBookingRepository = new Mock<IRepository<Booking>>();
            _mockRoomRepository = new Mock<IRepository<Room>>();

            _bookingManager = new BookingManager(_mockBookingRepository.Object, _mockRoomRepository.Object);
        }

        [Given(@"there is a hotel room available")]
        public void GivenThereIsAHotelRoomAvailable()
        {
            var rooms = new List<Room> { new Room { Id = 1 } };
            _mockRoomRepository.Setup(r => r.GetAll()).Returns(rooms);
        }

        [Given(@"the room is not booked from ""(.*)"" to ""(.*)""")]
        public void GivenTheRoomIsNotBookedFromTo(DateTime startDate, DateTime endDate)
        {
            _mockBookingRepository.Setup(b => b.GetAll()).Returns(new List<Booking>().AsQueryable());
            _bookingStartDate = startDate;
            _bookingEndDate = endDate;
        }

        [Given(@"the room is booked from ""(.*)"" to ""(.*)""")]
        public void GivenTheRoomIsBookedFromTo(DateTime startDate, DateTime endDate)
        {
            var existingBooking = new Booking
            {
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                RoomId = 1
            };
            _mockBookingRepository.Setup(b => b.GetAll()).Returns(new List<Booking> { existingBooking }.AsQueryable());
        }

        [When(@"I book the room from ""(.*)"" to ""(.*)""")]
        public void WhenIBookTheRoomFromTo(DateTime startDate, DateTime endDate)
        {
            var booking = new Booking
            {
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                RoomId = 1
            };

            _bookingResult = _bookingManager.CreateBooking(booking);
        }



        [Then(@"the booking should be unsuccessful")]
        public void ThenTheBookingShouldBeUnsuccessful()
        {
            Assert.False(_bookingResult, "Expected booking to fail, but it succeeded.");
            _mockBookingRepository.Verify(b => b.Add(It.IsAny<Booking>()), Times.Never);
        }

        [Then(@"the booking should be successful")]
        public void ThenTheBookingShouldBeSuccessful()
        {
            Assert.True(_bookingResult, "Expected booking to succeed, but it failed.");
            _mockBookingRepository.Verify(b => b.Add(It.IsAny<Booking>()), Times.Once);
        }


    }
}
