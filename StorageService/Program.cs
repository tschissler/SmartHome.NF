using Microsoft.AspNetCore.Mvc;
using SharedContracts.RestDataPoints;
using StorageService;

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

app.MapPost("/addpvm3data", bool ([FromBody] List<PVM3RestDataPoint> pvM3RestDataPoints) =>
{
    StorageConnector.AddPVM3Data(pvM3RestDataPoints);
    return true;
})
.WithName("AddPVM3Data");

app.Run();
