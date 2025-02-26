using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWorkShiftDetailRepository _workShiftDetailRepository;
        public WorkShiftsController(IWorkShiftRepository workShiftRepository, IMapper mapper, IWorkShiftDetailRepository workShiftDetailRepository)
        {
            _workShiftRepository = workShiftRepository;
            _workShiftDetailRepository = workShiftDetailRepository;
            _mapper = mapper;
        }

        // Fetching all work shifts
        [HttpGet]
        [Route("GetAllWorkShifts")]
        public async Task<IActionResult> GetAllWorkShifts(int page = 1, int pageSize=20, bool fetchAll= false)
        {
            try
            {
                if (fetchAll)
                {
                    var workShifts = await _workShiftRepository.GetAllAsync(query => query.Include(x => x.WorkShiftDetails));
                    if (!workShifts.Any())
                    {
                        return NotFound(new WorkShiftValidationRepsonse(false, "No work shifts found"));
                    }

                    var result = _mapper.Map<IEnumerable<ListWorkShiftDto>>(workShifts);
                    return Ok(new WorkShiftValidationRepsonse(true, "All work shifts fetched successfully",
                        null,
                        result));
                }
                else
                {
                    var workShifts = await _workShiftRepository.GetAllAsync(page, pageSize, query => query.Include(x => x.WorkShiftDetails));
                    if (!workShifts.Items.Any())
                    {
                        return NotFound(new WorkShiftValidationRepsonse(false, "No work shifts found"));
                    }

                    var result = _mapper.Map<IEnumerable<ListWorkShiftDto>>(workShifts.Items);
                    return Ok(new WorkShiftValidationPaginatedRepsonse(true, "All work shifts fetched successfully",
                        currentPage: page,
                        pageSize: pageSize,
                        totalCount: workShifts.TotalCount,
                        null,
                        result));

                }
                
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
        [HttpGet]
        [Route("GetDaysofTheWeek")]
        public IActionResult GetDaysofTheWeek()
        {
            try
            {
                // Get enum values from ShiftType enum
                var daysOftheWeek = Enum.GetValues(typeof(DayOfWeek))
                                     .Cast<DayOfWeek>()
                                     .Select(e => new { id = (int)e, name = e.ToString() })
                                     .ToList();

                return Ok(daysOftheWeek);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching shift types: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetWorkShiftById/{id}")]
        public async Task<IActionResult> GetWorkShiftById(int id)
        {
            try
            {
                var workShift = await _workShiftRepository.GetByIdAsync(id, query => query.Include(x => x.WorkShiftDetails));
                if (workShift == null)
                {
                    return NotFound(new UserValidationResponse(false, "Work shift not found"));
                }
                var result = _mapper.Map<ListWorkShiftDto>(workShift);
                return Ok(new WorkShiftValidationRepsonse(true, "Work shift fetched successfully", null, new List<ListWorkShiftDto> { result }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work shift by ID: {ex.Message}");
            }
        }

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

                // Validate ShiftType
                if (!Enum.IsDefined(typeof(ShiftType), updateWorkShiftDto.ShiftType))
                {
                    return BadRequest("Invalid ShiftType provided.");
                }
                var workShift = _mapper.Map<WorkShift>(updateWorkShiftDto);

                // Update default shift values or set as complex
                if (updateWorkShiftDto.IsComplex)
                {
                    // Update WorkShiftDetails
                    if (updateWorkShiftDto.WorkShiftDetails == null || !updateWorkShiftDto.WorkShiftDetails.Any())
                    {
                        return BadRequest("WorkShiftDetails are required for complex shifts.");
                    }

                    var workShiftDetails = await _workShiftDetailRepository.GetByWorkShiftIdAsync(existingWorkShift.Id);
                    foreach (var detailDto in updateWorkShiftDto.WorkShiftDetails)
                    {
                        var existingDetail = workShiftDetails.FirstOrDefault(d => d.Day.ToString() == detailDto.Day);
                        if (existingDetail != null)
                        {
                            detailDto.Id = existingDetail.Id;
                        }
                    }

                    workShift.WorkShiftDetails = _mapper.Map<ICollection<WorkShiftDetail>>(updateWorkShiftDto.WorkShiftDetails);
                }

                // Update the WorkShift in the repository
                await _workShiftRepository.UpdateAsync(existingWorkShift,workShift);
                return Ok(new UserValidationResponse(true, "Work shift updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating work shift: {ex.Message}");
            }
        }

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

                // If the work shift is complex, delete the related WorkShiftDetails
                if (workShift.IsComplex)
                {
                    // First delete the related WorkShiftDetails
                    var workShiftDetails = await _workShiftDetailRepository.GetByWorkShiftIdAsync(workShift.Id);
                    await _workShiftDetailRepository.DeleteRangeAsync(workShiftDetails);
                }

                // Now delete the work shift itself
                await _workShiftRepository.DeleteAsync(id);
                return Ok(new UserValidationResponse(true, "Work shift deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("CreateWorkShift")]
        public async Task<IActionResult> CreateWorkShift([FromBody] CreateorUpdateWorkShiftDto createWorkShiftDto)
        {
            try
            {
                if (createWorkShiftDto == null)
                {
                    return BadRequest("Work shift data is required.");
                }

                // Validate ShiftType
                if (!Enum.IsDefined(typeof(ShiftType), createWorkShiftDto.ShiftType))
                {
                    return BadRequest("Invalid ShiftType provided.");
                }
               
                var workShift = _mapper.Map<WorkShift>(createWorkShiftDto);
             

                // For complex shifts, validate and add WorkShiftDetails
                if (createWorkShiftDto.IsComplex)
                {
                    if (createWorkShiftDto.WorkShiftDetails == null || !createWorkShiftDto.WorkShiftDetails.Any())
                    {
                        return BadRequest("WorkShiftDetails are required for complex shifts.");
                    }
                    foreach (var detail in createWorkShiftDto.WorkShiftDetails)
                    {
                        if (!Enum.TryParse(typeof(DayOfWeek), detail.Day, true, out _))
                        {
                            return BadRequest($"Invalid day of the week");
                        }
                    }

                    workShift.WorkShiftDetails = _mapper.Map<ICollection<WorkShiftDetail>>(createWorkShiftDto.WorkShiftDetails);
                }

                // Save to database
                await _workShiftRepository.AddAsync(workShift);
                return Ok(new UserValidationResponse(true, "Work shift created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating work shift: {ex.Message}");
            }
        }

    }

}
