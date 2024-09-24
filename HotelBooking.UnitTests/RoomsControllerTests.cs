using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class RoomsControllerTests
    {
        private RoomsService roomsService;
        private Mock<IRepository<Room>> mockRoomRepo;

        public RoomsControllerTests()
        {
            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };

            // Create fake RoomRepository. 
            mockRoomRepo = new Mock<IRepository<Room>>();

            // Implement fake GetAll() method.
            mockRoomRepo.Setup(x => x.GetAll()).Returns(rooms);


            // Implement fake Get() method.
            //fakeRoomRepository.Setup(x => x.Get(2)).Returns(rooms[1]);


            // Alternative setup with argument matchers:

            // Any integer:
            //fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(rooms[1]);

            // Integers from 1 to 2 (using a predicate)
            // If the fake Get is called with an another argument value than 1 or 2,
            // it returns null, which corresponds to the behavior of the real
            // repository's Get method.
            //fakeRoomRepository.Setup(x => x.Get(It.Is<int>(id => id > 0 && id < 3))).Returns(rooms[1]);

            // Integers from 1 to 2 (using a range)
            mockRoomRepo.Setup(x => x.GetAll()).Returns(rooms);
            mockRoomRepo.Setup(x => x.Get(It.IsInRange<int>(1, 2, Moq.Range.Inclusive))).Returns(rooms[1]);

            // Create RoomsController
            roomsService = new RoomsService(mockRoomRepo.Object);
        }

        [Fact]
        public void GetAll_ReturnsListWithCorrectNumberOfRooms()
        {
            // Act
            var result = roomsService.GetAllRooms() as List<Room>;
            var noOfRooms = result.Count;

            // Assert
            Assert.Equal(2, noOfRooms);
        }

        [Fact]
        public void GetById_RoomExists_ReturnsIActionResultWithRoom()
        {
            // Act
            var room = roomsService.GetRoom(2);

            // Assert
            Assert.NotNull(room);
            Assert.Equal(2, room.Id);
        }

        [Fact]
        public void Delete_WhenIdIsLargerThanZero_RemoveIsCalled()
        {
            // Act
            roomsService.RemoveRoom(1);

            // Assert
            mockRoomRepo.Verify(x => x.Remove(1), Times.Once);
        }


        [Fact]
        public void Delete_WhenIdIsLessThanOne_RemoveIsNotCalled()
        {
            // Act && Assert
            var exception = Assert.Throws<InvalidOperationException>(() => roomsService.RemoveRoom(0));
            Assert.IsType<InvalidOperationException>(exception);


            // Assert against the mock object
            mockRoomRepo.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Delete_WhenIdDoesNotExist_RemoveThrowsException()
        {
            // Arrange
            mockRoomRepo.Setup(x => x.Get(It.IsAny<int>())).Returns((Room)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => roomsService.RemoveRoom(3));
    
            // Verify that the exception is of the correct type
            Assert.IsType<InvalidOperationException>(exception);

            // Verify that Remove was never called since the room does not exist
            mockRoomRepo.Verify(x => x.Remove(It.IsAny<int>()), Times.Never());
        }

    }
}
