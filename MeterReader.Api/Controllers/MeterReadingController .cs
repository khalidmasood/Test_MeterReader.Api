using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MeterReader.Api.Data;
using CsvHelper;
using MeterReader.Api.Models;
using CsvHelper.Configuration;
using MeterReader.Api.Services;

namespace MeterReader.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {

        private readonly ILogger<MeterReadingController> _logger;
        private readonly IMeterReadingService _meterReadingService;

        public MeterReadingController(IMeterReadingService meterReadingService, ILogger<MeterReadingController> logger)
        {
            _logger = logger;
            _meterReadingService = meterReadingService;
        }


        [HttpPost("meter-reading-uploads")]
        public async Task<IActionResult> UploadMeterReadings(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try {

                var result = _meterReadingService.UploadMeterReadings(file).Result;

                return Ok(new { SuccessfulCount = result.Item1, FailedCount = result.Item2});
            }


            catch (Exception ex){

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }


       
    }

}