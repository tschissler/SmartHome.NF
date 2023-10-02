using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PowerDogLib;
using PVService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

Dictionary<string, string> sensorKeys = new()
            {
                { "Bezug", "iec1107_1457430339" }, // Vom Z�hler
                { "Erzeugung", "pv_global_1454412642" },  // Vom Wechselrichter
                { "Eigenverbrauch", "arithmetic_1457431399" },
                { "Verbrauchgesamt", "arithmetic_1457432629" },
                { "lieferung", "iec1107_1457430562" } // Vom Z�hler
            };


PowerDog powerDog = null;
try
{
    powerDog = new PowerDog(sensorKeys, new UriBuilder("http", "powerdog", 20000).Uri, SharedContracts.Configuration.PowerDog.Password);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

var pvStorageConnector = new PVStorageConnector(powerDog);

TimeSpan readDeviceDataInterval = TimeSpan.FromSeconds(2);
TimeSpan sendDataToCloudInterval = TimeSpan.FromSeconds(10);

var refreshDataTimer = new Timer(new TimerCallback(powerDog.ReadSensorsData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);
var sendDataToCloudTimer = new Timer(new TimerCallback(pvStorageConnector.SendDataToCloud), null, 0, (int)sendDataToCloudInterval.TotalMilliseconds);

app.MapGet("/readSensorsdata", string () =>
{
    lock (powerDog.Lockobject)
    {
        return JsonConvert.SerializeObject(powerDog.LocalDataPoints);
    }
})
.WithName("ReadSensorsData");

app.MapGet("/ping", string () =>
{
        return "I am alive!";
})
.WithName("Ping");

app.Run();
