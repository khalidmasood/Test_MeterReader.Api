using MeterReader.Api.Controllers;
using MeterReader.Api.Repositories;
using MeterReader.Api.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System.Runtime.CompilerServices;
using MeterReader.Api.Models;
using MeterReader.Api.Data;
using Newtonsoft.Json.Linq;

namespace MeterReader.Api.Test
{
    public class MeterReadingServiceTests
    {

        //private readonly Mock<IConsumerRepository> _consumerReporsitoryMock = new Mock<IConsumerRepository>();
        private readonly IConsumerRepository _consumerReporsitory;
        private readonly Mock<ILogger<MeterReadingService>> _logger = new Mock<ILogger<MeterReadingService>>();


        private ConsumerDbContext CreateTestDbContext()
        {
            var options = new DbContextOptionsBuilder<ConsumerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Unique in-memory database
                .Options;

            var context = new ConsumerDbContext(options);

            // Add test data
            context.MeterReadings.AddRange(
                new MeterReading { AccountId = 1, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 10000 },
                new MeterReading { AccountId = 2, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 20000 },
                new MeterReading { AccountId = 3, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 30000 },
                new MeterReading { AccountId = 4, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 40000 },
                new MeterReading { AccountId = 5, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 50000 }
            );

            context.ConsumerAccounts.AddRange(
                new ConsumerAccount { AccountId = 1, FirstName = "Khalid", LastName = "Wasti"},
                new ConsumerAccount { AccountId = 2, FirstName = "Stuart", LastName = "Aubry" },
                new ConsumerAccount { AccountId = 3, FirstName = "Simon", LastName = "Murphy" },
                new ConsumerAccount { AccountId = 4, FirstName = "Raul", LastName = "Shiltz" }
                );


            context.SaveChanges();  // Ensure the test data is saved

            return context;
        }



        public MeterReadingServiceTests()
        {

            var dbContext = CreateTestDbContext();

            _consumerReporsitory = new ConsumerRepository(dbContext);

        }

        [Fact]
        public async Task UploadMeterReadings_ShouldPass_ValidEntry() //Invalid account check
        {


            //Arrange
            var meterReadingService = new MeterReadingService(_consumerReporsitory, _logger.Object);

            var existingMeterReading = new List<MeterReading>() {
                new Models.MeterReading() { AccountId = 1, MeterReadingDateTime = new DateTime(2024, 12, 1), MeterReadValue = 10001 }
            };


            //Action
            var result = await meterReadingService.SaveMeterReadings(existingMeterReading);

            //Assert

            Assert.True(result.Item1 == 1 && result.Item2 == 0);

        }


        [Fact]
        public async Task UploadMeterReadings_ShouldFail_ForInvalidAccount() //Invalid account check
        {


            //Arrange
            var meterReadingService = new MeterReadingService(_consumerReporsitory, _logger.Object);

            var existingMeterReading = new List<MeterReading>() { 
                new Models.MeterReading() { AccountId = 5, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 10000 } 
            };


            //Action
            var result = await meterReadingService.SaveMeterReadings(existingMeterReading);

            //Assert

            Assert.True(result.Item1 == 0 && result.Item2 == 1);



        }

        [Fact]
        public async Task UploadMeterReadings_ShouldFail_WhenReadingExists() //Duplicate test
        {


            //Arrange
            var meterReadingService = new MeterReadingService(_consumerReporsitory, _logger.Object);

            var existingMeterReading = new List<MeterReading>() {
                new Models.MeterReading() { AccountId = 1, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 10000 }
            };


            //Action
            var result = await meterReadingService.SaveMeterReadings(existingMeterReading);

            //Assert

            Assert.True(result.Item1 == 0 && result.Item2 == 1);
        }


        [Fact]
        public async Task UploadMeterReadings_ShouldFail_Reading_Invalid_Format_NNNNN() //Invalid NNNNN format test
        {


            //Arrange
            var meterReadingService = new MeterReadingService(_consumerReporsitory, _logger.Object);

            var existingMeterReading = new List<MeterReading>() {
                new Models.MeterReading() { AccountId = 1, MeterReadingDateTime = new DateTime(2024, 11, 30), MeterReadValue = 100000 }
            };


            //Action
            var result = await meterReadingService.SaveMeterReadings(existingMeterReading);

            //Assert

            Assert.True(result.Item1 == 0 && result.Item2 == 1);



        }

        [Fact]
        public async Task UploadMeterReadings_ShouldFail_Reading_Not_Latest() //Older Entery Test
        {


            //Arrange
            var meterReadingService = new MeterReadingService(_consumerReporsitory, _logger.Object);

            var existingMeterReading = new List<MeterReading>() {
                new Models.MeterReading() { AccountId = 3, MeterReadingDateTime = new DateTime(2024, 11, 29), MeterReadValue = 10000 }
            };


            //Action
            var result = await meterReadingService.SaveMeterReadings(existingMeterReading);

            //Assert

            Assert.True(result.Item1 == 0 && result.Item2 == 1);



        }



    }
}