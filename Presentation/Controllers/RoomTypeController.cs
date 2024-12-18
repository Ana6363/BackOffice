using BackOffice.Application.RoomType;
using BackOffice.Domain.RoomTypes;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/roomtype")]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly RoomTypeService _roomTypeService;

        public RoomTypeController(IRoomTypeRepository roomTypeRepository, RoomTypeService roomTypeService)
        {
            _roomTypeRepository = roomTypeRepository;
            _roomTypeService = roomTypeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoomTypeAsync([FromBody] RoomTypeDto roomTypeDto)
        {
            if (roomTypeDto == null)
            {
                return BadRequest(new { success = false, message = "Room type details are required." });
            }
            try
            {
                await _roomTypeService.CreateRoomType(roomTypeDto);
                return Ok(new { success = true, message = "Room type created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
