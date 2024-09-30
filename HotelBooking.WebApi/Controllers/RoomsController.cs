using System;
using System.Collections.Generic;
using HotelBooking.Core;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: rooms
        [HttpGet(Name = "GetRooms")]
        public IEnumerable<Room> Get()
        {
            return _roomService.GetAllRooms();
        }

        // GET rooms/5
        [HttpGet("{id}", Name = "GetRoomById")]
        public IActionResult Get(int id)
        {
            var item = _roomService.GetRoom(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }


        // POST roooms
        [HttpPost]
        public IActionResult Post([FromBody] Room room)
        {
            if (room == null)
            {
                return BadRequest();
            }

            _roomService.AddRoom(room);

            return CreatedAtAction(nameof(Get), new { id = room.Id }, room);
        }



        // DELETE rooms/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                _roomService.RemoveRoom(id);
                return NoContent();
            }
            else {
                return BadRequest();
            }
        }

    }
}
