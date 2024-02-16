using LoggingAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// dodaj rabbitMQ
builder.Services.AddSingleton<RabbitMQService>();

builder.Services.AddSingleton<MongoDbContext>(sp => new MongoDbContext(builder.Configuration));
builder.Services.AddScoped<LogDatabaseService>();

// Retrieve the environment variable indicating whether the app is in development mode
string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Define the frontend URL based on the environment
string frontendUrl;
if (environment == "Development")
{
    frontendUrl = "http://localhost:11180"; // Use localhost in development mode
}
else
{
    frontendUrl = "https://userinterface:11180"; // Use your production URL in other environments
}

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins(frontendUrl) // Add your frontend URL here
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowSpecificOrigin");

app.Run();
