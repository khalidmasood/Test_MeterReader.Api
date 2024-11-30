using Microsoft.EntityFrameworkCore;
using MeterReader.Api.Data;
using Serilog;
using MeterReader.Api.Services;
using MeterReader.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().MinimumLevel.Warning().WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day).CreateLogger();


builder.Host.UseSerilog();

builder.Logging.AddConsole();


var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddDbContext<ConsumerDbContext>(options =>
        options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
builder.Services.AddScoped<IConsumerRepository, ConsumerRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


