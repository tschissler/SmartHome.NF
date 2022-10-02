using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVController;
using Secrets;

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

var powerDog = new PowerDog(sensorKeys, new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password);
TimeSpan readDeviceDataInterval = TimeSpan.FromSeconds(1);

var refreshDataTimer = new Timer(new TimerCallback(powerDog.ReadSensorsData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);

app.MapGet("/readSensorsdata", string () =>
{
    return JsonConvert.SerializeObject(powerDog.DataPoints);
})
.WithName("ReadSensorsData");

app.Run();