using MeterReader.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeterReader.Api.Services
{
    public interface IMeterReadingService
    {
        public Task<(int, int)> UploadMeterReadings(IFormFile file);

        public Task<(int, int)> SaveMeterReadings(IList<MeterReading> meterReadings);

    }
}
