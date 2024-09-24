using System;
using System.Collections.Generic;

namespace HotelBooking.Core
{
    public class RoomsService : IRoomService
    {
        private readonly IRepository<Room> roomRepository;

        public RoomsService(IRepository<Room> roomRepository)
        {
            this.roomRepository = roomRepository;
        }
        
        public Room GetRoom(int id)
        {
            if (id <= 0)
            {
                throw new InvalidOperationException($"Room ID must be greater than zero");
            }
            
            var room = roomRepository.Get(id);
            if (room == null)
            {
                throw new InvalidOperationException($"Room with ID {id} does not exist");
            }

            return roomRepository.Get(id);
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return roomRepository.GetAll();
        }

        public void AddRoom(Room room)
        {
            // null check
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room), "Room cannot be null.");
            }
            
            // check if id is valid
            if (room.Id <= 0)
            {
                throw new InvalidOperationException($"Room ID must be greater than zero.");
            }
            
            // check if duplicate
            var existingRoom = roomRepository.Get(room.Id);
            if (existingRoom != null)
            {
                throw new InvalidOperationException($"Room With the Id {room.Id} allready added");
            }
            
            roomRepository.Add(room);
        }

        public void RemoveRoom(int id)
        {
            // check if greater than 0
            if (id <= 0)
            {
                throw new InvalidOperationException($"Room with ID {id} does not exist");
            }
            
            var room = roomRepository.Get(id);
            // check if null
            if (room == null)
            {
                throw new InvalidOperationException($"Room with ID {id} does not exist.");
            }
            
            roomRepository.Remove(id);
        }

        public void EditRoom(Room room)
        {
            // Check if null
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room), "Room cannot be null.");
            }
            
            // Check if Id is valid
            if (room.Id <= 0)
            {
                throw new InvalidOperationException($"Room ID must be greater than zero.");
            }
            
            // Check if a room with the given id exists
            var existingRoom = roomRepository.Get(room.Id);
            if (existingRoom == null)
            {
                throw new InvalidOperationException($"Room With {room.Id} does not exist");
            }
            
            roomRepository.Edit(room);
        }
    }
}