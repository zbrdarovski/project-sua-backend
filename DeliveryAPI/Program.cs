// Program.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dostava API",
        Version = "v1"
    });
});

builder.Services.AddHealthChecks();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:44459")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var configuration = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

    if (configuration?.ConnectionString is null)
    {
        throw new ArgumentNullException(nameof(configuration.ConnectionString), "MongoDB connection string is missing in configuration.");
    }

    if (configuration?.DatabaseName is null)
    {
        throw new ArgumentNullException(nameof(configuration.DatabaseName), "MongoDB database name is missing in configuration.");
    }

    return new MongoDbContext(configuration.ConnectionString, configuration.DatabaseName);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dostava API"));
}

app.UseHttpsRedirection();

app.MapPost("/api/deliveries", (DeliveryCreateDto deliveryDto, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = new Delivery
    {
        UserId = deliveryDto.UserId,
        PaymentId = deliveryDto.PaymentId,
        Address = deliveryDto.Address,
        DeliveryTime = deliveryDto.DeliveryTime,
        GeoX = deliveryDto.GeoX,
        GeoY = deliveryDto.GeoY
    };

    dbContext.Deliveries.InsertOne(delivery);
    return Results.Ok(new { Message = "Delivery added successfully" });
}).WithName("AddDelivery");

app.MapGet("/api/deliveries", ([FromServices] MongoDbContext dbContext) =>
{
    var deliveries = dbContext.Deliveries.Find(_ => true).ToList();
    return Results.Ok(deliveries);
}).WithName("GetAllDeliveries");

app.MapGet("/api/deliveries/{id}", (string id, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = dbContext.Deliveries.Find(d => d.Id == id).FirstOrDefault();
    if (delivery == null)
    {
        return Results.NotFound(new { Message = "Delivery not found" });
    }

    return Results.Ok(delivery);
}).WithName("GetDeliveryById");

app.MapPut("/api/deliveries/{id}", (string id, DeliveryUpdateDto deliveryDto, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = dbContext.Deliveries.Find(d => d.Id == id).FirstOrDefault();

    if (delivery == null)
    {
        return Results.NotFound(new { Message = "Delivery not found" });
    }

    delivery.Address = deliveryDto.Address;
    delivery.GeoX = deliveryDto.GeoX;
    delivery.GeoY = deliveryDto.GeoY;

    dbContext.Deliveries.ReplaceOne(d => d.Id == id, delivery);

    return Results.Ok(new { Message = "Delivery updated successfully" });
}).WithName("UpdateDelivery");

app.MapDelete("/api/deliveries/{id}", (string id, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = dbContext.Deliveries.Find(d => d.Id == id).FirstOrDefault();
    if (delivery == null)
    {
        return Results.NotFound(new { Message = "Delivery not found" });
    }

    dbContext.Deliveries.DeleteOne(d => d.Id == id);

    return Results.Ok(new { Message = "Delivery deleted successfully" });
}).WithName("DeleteDelivery");

// Additional CRUD Operations

app.MapPut("/api/deliveries/update-coordinates/{id}", (string id, CoordinatesUpdateDto coordinatesDto, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = dbContext.Deliveries.Find(d => d.Id == id).FirstOrDefault();

    if (delivery == null)
    {
        return Results.NotFound(new { Message = "Delivery not found" });
    }

    delivery.GeoX = coordinatesDto.GeoX;
    delivery.GeoY = coordinatesDto.GeoY;

    dbContext.Deliveries.ReplaceOne(d => d.Id == id, delivery);

    return Results.Ok(new { Message = "Coordinates updated successfully" });
}).WithName("UpdateCoordinates");

app.MapPut("/api/deliveries/update-delivery-time/{id}", (string id, DeliveryTimeUpdateDto deliveryTimeDto, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = dbContext.Deliveries.Find(d => d.Id == id).FirstOrDefault();

    if (delivery == null)
    {
        return Results.NotFound(new { Message = "Delivery not found" });
    }

    delivery.DeliveryTime = deliveryTimeDto.DeliveryTime;

    dbContext.Deliveries.ReplaceOne(d => d.Id == id, delivery);

    return Results.Ok(new { Message = "Delivery time updated successfully" });
}).WithName("UpdateDeliveryTime");

app.MapPut("/api/deliveries/update-address/{id}", (string id, UpdateAddressDto addressDto, [FromServices] MongoDbContext dbContext) =>
{
    var delivery = dbContext.Deliveries.Find(d => d.Id == id).FirstOrDefault();

    if (delivery == null)
    {
        return Results.NotFound(new { Message = "Delivery not found" });
    }

    delivery.Address = addressDto.Address;

    dbContext.Deliveries.ReplaceOne(d => d.Id == id, delivery);

    return Results.Ok(new { Message = "Address updated successfully" });
}).WithName("UpdateAddress");

app.UseHealthChecks("/api/deliveries/health");

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.Run();