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


