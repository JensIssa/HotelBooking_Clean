using System.Collections.Generic;

namespace HotelBooking.Core
{
    public interface IRoomService
    {
        Room GetRoom(int id);
        IEnumerable<Room> GetAllRooms();
        void AddRoom(Room room);
        void RemoveRoom(int id);
        void EditRoom(Room room);
    }
}