using BackOffice.Application.OperationTypes;
using BackOffice.Domain.OperationType;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("api/operationType")]

    public class OperationTypeController : ControllerBase
    {
        private readonly OperationTypeService _operationTypeService;

        private readonly IOperationTypeRepository _operationTypeRepository;

        private readonly IConfiguration _configuration;

        public OperationTypeController(OperationTypeService operationTypeService, IOperationTypeRepository operationTypeRepository, IConfiguration configuration)
        {
            _operationTypeService = operationTypeService;
            _operationTypeRepository = operationTypeRepository;
            _configuration = configuration;
        }

        [HttpPost("Create")]

            public async Task<IActionResult> Create([FromBody] OperationTypeDTO operationTypeDTO)
            {
                if (operationTypeDTO == null)
                {
                    return BadRequest(new { success = false, message = "Operation type details required"});
                }

                try
                {
                    var operationTypeDataModel = await _operationTypeService.CreateOperationType(operationTypeDTO);

                    return Ok(new { success = true, operationType = operationTypeDataModel});
                } catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }

                
            }

        [HttpPut("Update")]

            public async Task<IActionResult> Update([FromBody] OperationTypeDTO operationTypeDTO)
            {
                if (operationTypeDTO == null)
                {
                    return BadRequest("OperationType data cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(operationTypeDTO.OperationTypeId))
                {
                    return BadRequest("OperationType ID cannot be null or empty.");
                }

                try
                {
                    var updatedOperationType = await _operationTypeService.UpdateAsync(operationTypeDTO);

                    if (updatedOperationType == null)
                    {
                        return NotFound($"OperationType with ID '{operationTypeDTO.OperationTypeId}' not found.");
                    }
                     
                    return Ok(updatedOperationType);

                } catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }

            }


        
        [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(string id)
            {
                var deleted = await _operationTypeService.DeleteOperationTypeAsync(id);
                if (!deleted)
                {
                    return NotFound($"OperationType with ID {id} not found.");
                }
                return NoContent();
            }


        [HttpGet("GetAllOperationTypes")]
            public async Task<IActionResult> GetAll()
            {
                try
                {
                    var operationType = await _operationTypeService.GetAllAsync();
                    return Ok(new { success = true, operationType });
                } catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }
                

            } 

        [HttpGet("GetByID/{id}")]       
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("OperationType ID cannot be null or empty.");
            }

            try
            { 
                var operationId = new OperationTypeId(id);
                var operationTypeDataModel = await _operationTypeService.GetByIdAsync(operationId);
            
                return Ok(new { success = true, operationType = operationTypeDataModel});
            } catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}