using BackOffice.Application.OperationRequest;
using BackOffice.Domain.OperationRequest;
using BackOffice.Infraestructure.OperationRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("operationRequest")]
    public class OperationRequestController : ControllerBase 
    {
        private readonly OperationRequestService _operationRequestService;
        private readonly IOperationRequestRepository _operationRequestRepository;

        public OperationRequestController(OperationRequestService operationRequestService, IOperationRequestRepository operationRequestRepository)
        {
            _operationRequestService = operationRequestService;
            _operationRequestRepository = operationRequestRepository;
        }

        [HttpPost("create")]
        //[Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateOperationRequestAsync([FromBody] OperationRequestDto operationRequest)
        {
            if (operationRequest == null)
            {
                return BadRequest(new { success = false, message = "Operation request details are required." });
            }

            try
            {
                var operationRequestDataModel = await _operationRequestService.CreateOperationRequestAsync(operationRequest);
                return Ok(new { success = true, operationRequest = operationRequestDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateOperationRequestAsync([FromBody] OperationRequestDto operationRequest)
        {
            if (operationRequest == null)
            {
                return BadRequest(new { success = false, message = "Operation request details are required." });
            }

            try
            {
                var operationRequestDataModel = await _operationRequestService.UpdateAsync(operationRequest);
                return Ok(new { success = true, operationRequest = operationRequestDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteOperationRequestAsync([FromBody] OperationRequestDto operationRequest)
        {
            if (operationRequest == null)
            {
                return BadRequest(new { success = false, message = "Operation request details are required." });
            }

            try
            {
                var operationRequestDataModel = await _operationRequestService.DeleteOperationRequestAsync(operationRequest);
                return Ok(new { success = true, operationRequest = operationRequestDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("filter")]
        //[Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAllOperationRequestsAsync(
            [FromQuery] string? fullname = null,
            [FromQuery] string? priority = null,
            [FromQuery] string? status = null,
            [FromQuery] string? operationTypeName = null)
        {
            try
            {
                var filterDto = new FilteredRequestDto(fullname,priority,status,operationTypeName);

                var operationRequests = await _operationRequestService.GetFilteredRequestAsync(filterDto);
                return Ok(new { success = true, operationRequests });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
    }
}
