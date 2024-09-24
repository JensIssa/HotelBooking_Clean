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
            return roomRepository.Get(id);
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return roomRepository.GetAll();
        }

        public void AddRoom(Room room)
        {
            roomRepository.Add(room);
        }

        public void RemoveRoom(int id)
        {
            if (id <= 0)
            {
                throw new InvalidOperationException($"Room with ID {id} does not exist");
            }
            
            var room = roomRepository.Get(id);
            if (room == null)
            {
                throw new InvalidOperationException($"Room with ID {id} does not exist.");
            }
            
            roomRepository.Remove(id);
        }

        public void EditRoom(Room room)
        {
            roomRepository.Edit(room);
        }
    }
}