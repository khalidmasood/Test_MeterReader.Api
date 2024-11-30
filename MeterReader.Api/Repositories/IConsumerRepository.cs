using MeterReader.Api.Data;
using MeterReader.Api.Models;
using System.Threading.Tasks;

namespace MeterReader.Api.Repositories
{
    public interface IConsumerRepository
    {

        public void AddMeterReading(MeterReading meterReading);

        public Task<bool> IsDuplicateMeterReading(MeterReading reading);
        public Task<bool> ConsumerAccountExists(MeterReading reading);

        public Task<bool> IsNewReadOlderThanExistingRead(MeterReading reading);

        public Task<int> CommitMeterReadings();

    }
}
