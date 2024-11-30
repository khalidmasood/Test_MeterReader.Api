using CsvHelper;
using CsvHelper.Configuration;
using MeterReader.Api.Controllers;
using MeterReader.Api.Models;
using MeterReader.Api.Repositories;
using System.Globalization;

namespace MeterReader.Api.Services
{
    public class MeterReadingService : IMeterReadingService
    {

        private readonly IConsumerRepository _consumerRepository;

        private readonly ILogger<MeterReadingService> _logger;


        public MeterReadingService(IConsumerRepository consumerRepository, ILogger<MeterReadingService> logger) {

            _consumerRepository = consumerRepository;
            _logger = logger;

        }

        public async Task<(int, int)> UploadMeterReadings(IFormFile file)
        {

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    //register a custom map
                    csv.Context.RegisterClassMap<ReadingDateMap>();

                    var meterReadings = csv.GetRecords<MeterReading>().ToList();

                    var results = await SaveMeterReadings(meterReadings);

                    return results;

                }

                

                //Ok(new { SuccessfulCount = successfulCount, FailedCount = failedCount });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);

                throw;

                
            }
        }

        public async Task<(int, int)> SaveMeterReadings(IList<MeterReading> meterReadings) {

            var successfulCount = 0;
            var failedCount = 0;

            foreach (var reading in meterReadings)
            {
                if (await IsValidReading(reading))
                {
                    // Save valid readings to the database
                    var meterReading = new MeterReading
                    {
                        AccountId = reading.AccountId,
                        MeterReadingDateTime = reading.MeterReadingDateTime,
                        MeterReadValue = reading.MeterReadValue
                    };

                    _consumerRepository.AddMeterReading(meterReading);

                    successfulCount++;
                }
                else
                {
                    failedCount++;
                }
            }

            // Commit transaction
            await _consumerRepository.CommitMeterReadings();

            return (successfulCount, failedCount);


        }

        //Domain logic validations
        private async Task<bool> IsValidReading(MeterReading reading)
        {
            var readStr = $"Meter reading {reading.MeterReadValue} on account {reading.AccountId} at {reading.MeterReadingDateTime.ToString("dd/MM/yy hh:ss")}";


            // Validate if AccountId exists in the CustomerAccounts table
            var accountExists = _consumerRepository.ConsumerAccountExists(reading).Result;
            if (!accountExists)
            {
                _logger.LogError($"Account doesn't exist for: {readStr}"); // A meter reading must be associated with an Account ID to be deemed valid

                return false;
            }

            // Validate the meter reading value format (NNNNN)
            if (!System.Text.RegularExpressions.Regex.IsMatch("" + reading.MeterReadValue, @"^\d{5}$"))
            {
                _logger.LogError($"Reading values should be in the format NNNNN: {readStr}"); //You should not be able to load the same entry twice
                return false;
            }

            // Check for duplicate entries: same AccountId and MeterReadingDateTime
            var duplicateExists = await _consumerRepository.IsDuplicateMeterReading(reading);

            if (duplicateExists)
            {
                _logger.LogError($"Duplicate reading for: {readStr}");
                return false;
            }

            //NICE TO HAVE
            var newEntryExists = await _consumerRepository.IsNewReadOlderThanExistingRead(reading);

            if (newEntryExists)
            {
                _logger.LogError($"Reading older than the latest reading on account: {readStr}");
                return false;
            }

            return true;
        }


        private class ReadingDateMap : ClassMap<MeterReading>
        {
            public ReadingDateMap()
            {
                //22/04/2019 09:24
                Map(m => m.MeterReadingDateTime).Name("MeterReadingDateTime").TypeConverterOption.Format("dd/MM/yyyy HH:mm");
                Map(m => m.AccountId).Name("AccountId");
                Map(m => m.MeterReadValue).Name("MeterReadValue");
            }
        }

    }
}
