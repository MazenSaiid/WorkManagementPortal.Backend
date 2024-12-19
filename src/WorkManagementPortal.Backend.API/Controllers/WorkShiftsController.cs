using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Enums;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WorkShiftsController : ControllerBase
    {
        private readonly IWorkShiftRepository _workShiftRepository;  // Repository for WorkShifts
        private readonly IMapper _mapper;  // AutoMapper for DTO mapping

        public WorkShiftsController(IWorkShiftRepository workShiftRepository, IMapper mapper)
        {
            _workShiftRepository = workShiftRepository ;
            _mapper = mapper;
        }

        // Fetching all work shifts
        [HttpGet]
        [Route("GetAllWorkShifts")]
        public async Task<IActionResult> GetAllWorkShifts()
        {
            try
            {
                var workShifts = await _workShiftRepository.GetAllAsync();
                if (!workShifts.Any())
                {
                    return NotFound(new UserValidationResponse(false, "No work shifts found"));
                }

                var result = _mapper.Map<IEnumerable<ListWorkShiftDto>>(workShifts);
                return Ok(new WorkShiftValidationRepsonse(true, "All work shifts fetched successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching all work shifts: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("GetShiftTypes")]
        public IActionResult GetShiftTypes()
        {
            try
            {
                // Get enum values from ShiftType enum
                var shiftTypes = Enum.GetValues(typeof(ShiftType))
                                     .Cast<ShiftType>()
                                     .Select(e => new { id = (int)e, name = e.ToString() })
                                     .ToList();

                return Ok(shiftTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching shift types: {ex.Message}");
            }
        }
        // Example of GET by ID for a work shift
        [HttpGet]
        [Route("GetWorkShiftById/{id}")]
        public async Task<IActionResult> GetWorkShiftById(int id)
        {
            try
            {
                var workShift = await _workShiftRepository.GetByIdAsync(id);
                if (workShift == null)
                {
                    return NotFound(new UserValidationResponse(false, "Work shift not found"));
                }
                var result = _mapper.Map<ListWorkShiftDto>(workShift);
                return Ok(new WorkShiftValidationRepsonse(true, "Work shift fetched successfully",new List<ListWorkShiftDto> { result }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work shift by ID: {ex.Message}");
            }
        }

        // Example of PUT (updating an existing work shift)
        [HttpPut]
        [Route("UpdateWorkShift/{id}")]
        public async Task<IActionResult> UpdateWorkShift(int id, [FromBody] CreateorUpdateWorkShiftDto updateWorkShiftDto)
        {
            try
            {
                if (updateWorkShiftDto == null || id <= 0)
                {
                    return BadRequest("Work shift data is incorrect");
                }

                var existingWorkShift = await _workShiftRepository.GetByIdAsync(id);
                if (existingWorkShift == null)
                {
                    return NotFound();
                }
                if (!Enum.IsDefined(typeof(ShiftType), updateWorkShiftDto.ShiftType))
                {
                    return BadRequest("Invalid ShiftType provided.");
                }
                var workshift = _mapper.Map<WorkShift>(updateWorkShiftDto);

                await _workShiftRepository.UpdateAsync(existingWorkShift, workshift);
                return Ok(new UserValidationResponse(true, "Work shift updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating work shift: {ex.Message}");
            }
        }

        // Example of DELETE (removing a work shift)
        [HttpDelete]
        [Route("DeleteWorkShift/{id}")]
        public async Task<IActionResult> DeleteWorkShift(int id)
        {
            try
            {
                var workShift = await _workShiftRepository.GetByIdAsync(id);
                if (workShift == null)
                {
                    return NotFound();
                }

                await _workShiftRepository.DeleteAsync(id); // Deletes the work shift
                return Ok(new UserValidationResponse(true, "Work shift deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Example of POST (creating a new work shift)
        [HttpPost]
        [Route("CreateWorkShift")]
        public async Task<IActionResult> CreateWorkShift([FromBody] CreateorUpdateWorkShiftDto createWorkShiftDto)
        {
            try
            {
                if (createWorkShiftDto == null)
                {
                    return BadRequest("Work shift data is incorrect");
                }
                if (!Enum.IsDefined(typeof(ShiftType), createWorkShiftDto.ShiftType))
                {
                    return BadRequest("Invalid ShiftType provided.");
                }
                var workshift = _mapper.Map<WorkShift>(createWorkShiftDto);
                await _workShiftRepository.AddAsync(workshift);
                return Ok(new UserValidationResponse(true, "Work shift created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating work shift: {ex.Message}");
            }
        }
    }

}
