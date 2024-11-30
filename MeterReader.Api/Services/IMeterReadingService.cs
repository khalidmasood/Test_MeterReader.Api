using Microsoft.AspNetCore.Mvc;

namespace MeterReader.Api.Services
{
    public interface IMeterReadingService
    {
        public Task<(int, int)> UploadMeterReadings(IFormFile file); 

    }
}
