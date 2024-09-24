using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using Moq;
using System.Collections.Generic;
using HotelBooking.Infrastructure.Repositories;
using System.Collections;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;

        public BookingManagerTests() {
            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            bookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);
        }



        /// <summary>
        /// Testing the FindAvailableRoom method in the BookingManager class, where we look at the case where the start date is not in the future.
        /// Should return an ArgumentException.
        /// Optimized it to use the InlineData attribute to pass the test data to the test method, and used Moq for the repository.
        /// </summary>
        /// <param name="daysFromToday">Days from today</param>
        [Theory]
        [InlineData(-1)] // Past date
        [InlineData(0)]  // Today
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(int daysFromToday)
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(daysFromToday);
            DateTime endDate = startDate.AddDays(1);

            // Act
            Action act = () => bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        /// <summary>
        /// Testing the FindAvailableRoom method in the BookingManager class, where we make sure that the method returns -1 if no room is available.
        /// Optimized it to use the InlineData attribute to pass the test data to the test method, and used Moq for the repository.
        /// </summary>
        /// <param name="daysFromToday"></param>
        [Theory]
        [InlineData(1)] // Tomorrow
        [InlineData(2)] // Day after tomorrow
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne(int daysFromToday)
        {
            //Arrange
            DateTime startDate = DateTime.Today.AddDays(daysFromToday);
            DateTime endDate = startDate.AddDays(1);

            var rooms = new List<Room> { new Room { Id = 1 }, new Room { Id = 2 } };
            mockRoomRepository.Setup(r => r.GetAll()).Returns(rooms);

            var bookings = new List<Booking>();
            mockBookingRepository.Setup(b => b.GetAll()).Returns(bookings.AsQueryable());

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.NotEqual(-1, roomId);
        }

        /// <summary>
        /// Find available room method in the BookingManager class, where we make sure that the method returns a room that is available.
        /// Returns a room that is available.
        /// </summary>
        /// <param name="daysFromToday"></param>
        [Theory]
        [InlineData(1)] // Tomorrow
        public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom(int daysFromToday)
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(daysFromToday);
            DateTime endDate = startDate.AddDays(1);

            var rooms = new List<Room>
            {
                new Room { Id = 1 },
                new Room { Id = 2 }
            };
            mockRoomRepository.Setup(r => r.GetAll()).Returns(rooms);

            var bookings = new List<Booking>
            {
                new Booking
                {
                    RoomId = 1,
                    StartDate = startDate.AddDays(-3),
                    EndDate = startDate.AddDays(-1),
                    IsActive = true
                },
                new Booking
                {
                    RoomId = 2,
                    StartDate = startDate.AddDays(2),
                    EndDate = startDate.AddDays(4),
                    IsActive = true
                }
            };
            mockBookingRepository.Setup(b => b.GetAll()).Returns(bookings.AsQueryable());

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            var overlappingBookings = bookings.Where(
                b => b.RoomId == roomId
                && b.IsActive
                && b.StartDate < endDate
                && b.EndDate > startDate);

            // Assert
            Assert.NotEqual(-1, roomId);
            Assert.Empty(overlappingBookings);
        }
    
        /// <summary>
        /// Testing the CreateBooking method in the BookingManager class, where we look at different cases of booking creation.
        /// Our test data is passed to the test method using the ClassData attribute, and we use Moq for the repository.
        /// Our test cases can be seen in the "CreateBookingTestData".
        /// </summary>
        /// <param name="bookingStartDate">Start date of booking</param>
        /// <param name="bookingEndDate">End date of booking</param>
        /// <param name="existingBookings">Existing bookings</param>
        /// <param name="expectedResult">The expected return value of the method</param>

        [Theory]
        [ClassData(typeof(CreateBookingTestData))]
        public void CreateBooking_ReturnsExpectedResult(DateTime bookingStartDate, DateTime bookingEndDate, List<Booking> existingBookings, bool expectedResult)
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = bookingStartDate,
                EndDate = bookingEndDate,
                IsActive = true,
                CustomerId = 1,
                Id = 1
            };

            var rooms = new List<Room> { new Room { Id = 1 } };
            mockRoomRepository.Setup(r => r.GetAll()).Returns(rooms);

            mockBookingRepository.Setup(b => b.GetAll()).Returns(existingBookings.AsQueryable());

            // Act
            bool result = bookingManager.CreateBooking(booking);

            // Assert
            Assert.Equal(expectedResult, result);
            if (expectedResult)
            {
                mockBookingRepository.Verify(b => b.Add(It.IsAny<Booking>()), Times.Once);
            }
            else
            {
                mockBookingRepository.Verify(b => b.Add(It.IsAny<Booking>()), Times.Never);
            }
        }

        /// <summary>
        /// Testing the GetFullyOccupiedDates method in the BookingManager class, where we look at different cases of fully occupied dates.
        /// Our test data is passed to the test method using the ClassData attribute, and we use Moq for the repository.
        /// Our test cases can be seen in the "GetFullyOccupiedDatesTestData".
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="existingBookings">The existing bookings</param>
        /// <param name="rooms">List of rooms</param>
        /// <param name="expectedDates">List of expected fully occupied dates</param>

        [Theory]
        [ClassData(typeof(GetFullyOccupiedDatesTestData))]
        public void GetFullyOccupiedDates_ReturnsExpectedResult(DateTime startDate, DateTime endDate, List<Booking> existingBookings, List<Room> rooms, List<DateTime> expectedDates)
        {
            // Arrange
            mockRoomRepository.Setup(r => r.GetAll()).Returns(rooms);
            mockBookingRepository.Setup(b => b.GetAll()).Returns(existingBookings.AsQueryable());

            // Act
            var result = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expectedDates.OrderBy(d => d), result.OrderBy(d => d));
        }

        /// <summary>
        /// Checks whether a booking exists in the repository, before removing it.
        /// </summary>
        [Fact]
        public void RemoveBooking_BookingExists_RemovesBooking()
        {
            // Arrange
            int bookingId = 1;
            var booking = new Booking { Id = bookingId, IsActive = true };
            mockBookingRepository.Setup(b => b.Get(bookingId)).Returns(booking);
            mockBookingRepository.Setup(b => b.Remove(bookingId)).Verifiable();

            // Act
            bookingManager.RemoveBooking(bookingId);

            // Assert
            mockBookingRepository.Verify(b => b.Remove(bookingId), Times.Once);
        }


        /// <summary>
        /// Checks whether a booking exists in the repository, before modifying it.
        /// </summary>
        [Fact]
        public void EditBooking_BookingExists_EditsBooking()
        {
            // Arrange
            var existingBooking = new Booking { Id = 1, IsActive = true, CustomerId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), RoomId = 1 };
            var modifiedBooking = new Booking { Id = 1, IsActive = false, CustomerId = 2, StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(3), RoomId = 2 };

            mockBookingRepository.Setup(b => b.Get(existingBooking.Id)).Returns(existingBooking);
            mockBookingRepository.Setup(b => b.Edit(existingBooking)).Verifiable();

            // Act
            bookingManager.EditBooking(modifiedBooking);

            // Assert
            mockBookingRepository.Verify(b => b.Edit(existingBooking), Times.Once);

            Assert.Equal(existingBooking.IsActive, modifiedBooking.IsActive);
            Assert.Equal(existingBooking.CustomerId, modifiedBooking.CustomerId);
            Assert.Equal(existingBooking.StartDate, modifiedBooking.StartDate);
            Assert.Equal(existingBooking.EndDate, modifiedBooking.EndDate);
            Assert.Equal(existingBooking.RoomId, modifiedBooking.RoomId);
        }


        public class CreateBookingTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var today = DateTime.Today;

                // Test case 1: No overlapping bookings
                yield return new object[]
                {
                today.AddDays(10),
                today.AddDays(15),
                new List<Booking>(),
                true
                };

                // Test case 2: Overlapping booking
                yield return new object[]
                {
                today.AddDays(10),
                today.AddDays(15),
                new List<Booking>
                {
                    new Booking
                    {
                        RoomId = 1,
                        StartDate = today.AddDays(12),
                        EndDate = today.AddDays(17),
                        IsActive = true,
                        CustomerId = 2,
                        Id = 2
                    }
                },
                false
                };

                // Test case 3: Booking ends before existing booking starts
                yield return new object[]
                {
                today.AddDays(10),
                today.AddDays(12),
                new List<Booking>
                {
                    new Booking
                    {
                        RoomId = 1,
                        StartDate = today.AddDays(13),
                        EndDate = today.AddDays(15),
                        IsActive = true,
                        CustomerId = 2,
                        Id = 2
                    }
                },
                true
                };

                // Test case 4: Booking starts after existing booking ends
                yield return new object[]
                {
                today.AddDays(16),
                today.AddDays(18),
                new List<Booking>
                {
                    new Booking
                    {
                        RoomId = 1,
                        StartDate = today.AddDays(13),
                        EndDate = today.AddDays(15),
                        IsActive = true,
                        CustomerId = 2,
                        Id = 2
                    }
                },
                true
                };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

    public class GetFullyOccupiedDatesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var today = DateTime.Today;

            // Test case 1: No bookings
            yield return new object[]
            {
            today.AddDays(10),
            today.AddDays(14), 
            new List<Booking>(),
            new List<Room> { new Room { Id = 1 }, new Room { Id = 2 } },
            new List<DateTime>()
            };

            // Test case 2: All rooms occupied on certain dates
            yield return new object[]
            {
            today.AddDays(10), 
            today.AddDays(14),
            new List<Booking>
            {
                new Booking
                {
                    RoomId = 1,
                    StartDate = today.AddDays(10),
                    EndDate = today.AddDays(12),
                    IsActive = true,
                    CustomerId = 2,
                    Id = 2
                },
                new Booking
                {
                    RoomId = 2,
                    StartDate = today.AddDays(11),
                    EndDate = today.AddDays(13),
                    IsActive = true,
                    CustomerId = 3,
                    Id = 3
                }
            },
            new List<Room> { new Room { Id = 1 }, new Room { Id = 2 } },
            new List<DateTime>
            {
                today.AddDays(11),
                today.AddDays(12)
            }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


