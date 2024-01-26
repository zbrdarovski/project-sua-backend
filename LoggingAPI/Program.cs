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


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
