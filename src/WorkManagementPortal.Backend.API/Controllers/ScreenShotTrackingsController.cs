using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;
using Microsoft.IdentityModel.Tokens;
using WorkManagementPortal.Backend.Logic.Responses;
using WorkManagementPortal.Backend.Logic.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenShotTrackingsController : ControllerBase
    {
        private readonly IScreenShotRepository _screenShotRepository;

        public ScreenShotTrackingsController(IScreenShotRepository screenShotRepository)
        {
            _screenShotRepository = screenShotRepository;
        }

        [HttpPost("UploadScreenShot")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadScreenShot([FromForm] FileUploadDto fileUploadDto)
        {
            try
            {
                if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                if (string.IsNullOrEmpty(fileUploadDto.UserId))
                {
                    return BadRequest("User ID is required.");
                }

                await _screenShotRepository.UploadScreenShotAsync(fileUploadDto);

                return Ok(new ScreenShotValidationResponse(true, "Screenshots retrieved successfully.", null, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ScreenShotValidationResponse(false, "An error occurred while retrieving screenshots.", ex.Message));
            }
        }

        [HttpGet("GetScreenshotsForAllUsers")]
        public async Task<IActionResult> GetScreenshotsForAllUsers([FromQuery] DateTime date, int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            try
            {

                var data = await _screenShotRepository.GetScreenshotsForAllUsersAsync(date);
                if (data.Count == 0) 
                    return Ok(new ScreenShotValidationResponse(false, "No screenshots found."));
                if (fetchAll)
                {
                    return Ok(new ScreenShotValidationResponse(true, "Screenshots retrieved successfully.", null, data));
                }
                else
                {
                    var paginatedResult = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    var totalCount = data.Count;
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                    var response = new ScreenShotValidationPaginatedResponse(
                                   success: true,
                                   message: "User screenshots retrieved successfully.",
                                   currentPage: page,
                                   pageSize: pageSize,
                                   totalCount: totalCount,
                                   userScreenShotLogDto: paginatedResult);

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ScreenShotValidationResponse(false, "An error occurred while retrieving screenshots.", ex.Message));
            }
        }

        [HttpGet("GetScreenshotsForUser")]
        public async Task<IActionResult> GetScreenshotsForUser([FromQuery] string userId, [FromQuery] DateTime date, int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            try
            {

                var data = await _screenShotRepository.GetScreenshotsForUserAsync(userId, date);
                if (data.Count == 0) 
                    return Ok(new ScreenShotValidationResponse(false, "No screenshots found."));
                if (fetchAll)
                {
                    return Ok(new ScreenShotValidationResponse(true, "Screenshots retrieved successfully.", null, data));
                }
                else
                {
                    var paginatedResult = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    var totalCount = data.Count;
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                    var response = new ScreenShotValidationPaginatedResponse(
                                   success: true,
                                   message: $"User with id {userId} screenshots retrieved successfully.",
                                   currentPage: page,
                                   pageSize: pageSize,
                                   totalCount: totalCount,
                                   userScreenShotLogDto: paginatedResult);

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ScreenShotValidationResponse(false, "An error occurred while retrieving screenshots.", ex.Message));
            }
        }

    }
}
