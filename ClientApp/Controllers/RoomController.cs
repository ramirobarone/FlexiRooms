using Application.Interfaces;
using Application.Models;
using Application.Models.Booking.Available;
using Application.Services.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController(IServiceGeneric<RoomDto> serviceRoom, ILogger<RoomController> logger) : ControllerBase
    {
        [HttpGet]
        [Route(nameof(GetRoomById))]
        [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetRoomById(int idHotel)
        {
            logger.LogInformation("Getting rooms by hotel id {HotelId}", idHotel);

            if (idHotel == 0)
            {
                logger.LogWarning("Invalid hotel id received in GetRoomById: {HotelId}", idHotel);
                return NoContent();
            }

            IEnumerable<RoomDto> rooms = await serviceRoom.GetAllById(idHotel);

            if (rooms.Any())
            {
                logger.LogInformation("Found rooms for hotel id {HotelId}", idHotel);
                return Ok(rooms);
            }

            logger.LogInformation("No rooms found for hotel id {HotelId}", idHotel);
            return NoContent();
        }

        [HttpGet(nameof(GetRoom))]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRoom(int id)
        {
            logger.LogInformation("Getting room by id {RoomId}", id);

            if (id <= 0)
            {
                logger.LogWarning("Invalid room id received in GetRoom: {RoomId}", id);
                return BadRequest();
            }

            RoomDto room = await serviceRoom.GetById(id);
            if (room.Id == 0)
            {
                logger.LogInformation("Room not found for id {RoomId}", id);
                return NoContent();
            }

            logger.LogInformation("Room found for id {RoomId}", id);
            return Ok(room);
        }

        [HttpPost(nameof(CreateRoom))]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoom([FromBody] RoomDto roomDto)
        {
            logger.LogInformation("Creating room with name {RoomName}", roomDto.Name);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model received in CreateRoom");
                return BadRequest(ModelState);
            }

            RoomDto createdRoom = await serviceRoom.Create(roomDto);
            logger.LogInformation("Created room with id {RoomId}", createdRoom.Id);
            return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
        }

        [HttpPut(nameof(UpdateRoom))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRoom([FromBody] RoomDto roomDto)
        {
            logger.LogInformation("Updating room id {RoomId}", roomDto.Id);

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model received in UpdateRoom for room id {RoomId}", roomDto.Id);
                return BadRequest(ModelState);
            }

            await serviceRoom.Update(roomDto);
            logger.LogInformation("Updated room id {RoomId}", roomDto.Id);
            return Ok();
        }

        [HttpDelete(nameof(DeleteRoom))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            logger.LogInformation("Deleting room id {RoomId}", id);

            if (id <= 0)
            {
                logger.LogWarning("Invalid room id received in DeleteRoom: {RoomId}", id);
                return BadRequest();
            }

            await serviceRoom.Delete(id);
            logger.LogInformation("Deleted room id {RoomId}", id);
            return Accepted();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetTimes()
        {
            logger.LogInformation("Getting available room times");

            var result = await ((ITimeRoom)serviceRoom).GetTimesAsync();

            if (!result.Any())
            {
                logger.LogInformation("No available room times found");
                return NoContent();
            }

            logger.LogInformation("Available room times found");
            return Ok(result);
        }
    }
}
