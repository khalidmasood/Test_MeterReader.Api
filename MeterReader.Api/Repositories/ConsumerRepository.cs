using MeterReader.Api.Data;
using MeterReader.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReader.Api.Repositories
{
    public class ConsumerRepository : IConsumerRepository
    {

        private readonly ConsumerDbContext _context;

        public ConsumerRepository(ConsumerDbContext context)
        {
            _context = context;
        }


        public async void AddMeterReading(MeterReading reading) {

            await _context.MeterReadings.AddAsync(reading);
        }

        public async Task<bool> IsDuplicateMeterReading(MeterReading reading)
        {

            return await _context.MeterReadings.AnyAsync(mr => mr.AccountId == reading.AccountId && mr.MeterReadingDateTime == reading.MeterReadingDateTime);
        }


        public async Task<bool> ConsumerAccountExists(MeterReading reading)
        {
            return await _context.ConsumerAccounts.AnyAsync(c => c.AccountId == reading.AccountId);
        }

        public async void CommitMeterReadings() {

            await _context.SaveChangesAsync();

        }

    }
}
