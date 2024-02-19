using StatsHaramija;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<StatsRepository>(serviceProvider =>
{
    var mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
    return new StatsRepository(mongoDbConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StatsHaramija");
});

app.MapControllers();

app.Run();
