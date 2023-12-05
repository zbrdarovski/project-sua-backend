// Program.cs
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Uporabniska avtentikacija API",
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
            builder.WithOrigins("http://localhost:44459") // Add your frontend URL here
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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Uporabniska avtentikacija API"));
}

app.MapPost("/api/users/register", (UserRegistrationDto userDto, [FromServices] MongoDbContext dbContext) =>
{
    var user = new User { Id = userDto.Id, Username = userDto.Username, Password = userDto.Password };
    dbContext.Users.InsertOne(user);
    return Results.Ok(new { Message = "Registration successful" });
}).WithName("Register");

app.MapPost("/api/users/login", (UserLoginDto userDto, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Username == userDto.Username && u.Password == userDto.Password).FirstOrDefault();

    if (user == null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new { Message = "Login successful" });
}).WithName("Login");

app.MapPost("/api/users/change-password", (ChangePasswordDto changePasswordDto, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Username == changePasswordDto.Username && u.Password == changePasswordDto.CurrentPassword).FirstOrDefault();

    if (user == null)
    {
        return Results.Unauthorized();
    }

    user.Password = changePasswordDto.NewPassword;
    dbContext.Users.ReplaceOne(u => u.Id == user.Id, user);

    return Results.Ok(new { Message = "Password changed successfully" });
}).WithName("ChangePassword");

app.MapGet("/api/users", ([FromServices] MongoDbContext dbContext) =>
{
    var users = dbContext.Users.Find(_ => true).ToList();
    return Results.Ok(users);
}).WithName("GetAllUsers");

app.MapGet("/api/users/{id}", (string id, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Id == id).FirstOrDefault();
    if (user == null)
    {
        return Results.NotFound(new { Message = "User not found" });
    }

    return Results.Ok(user);
}).WithName("GetUserById");

app.MapPut("/api/users/{id}", (string id, UserUpdateDto userDto, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Id == id).FirstOrDefault();
    if (user == null)
    {
        return Results.NotFound(new { Message = "User not found" });
    }

    user.Username = userDto.Username;
    dbContext.Users.ReplaceOne(u => u.Id == id, user);

    return Results.Ok(new { Message = "User updated successfully" });
}).WithName("UpdateUser");

app.MapDelete("/api/users/{id}", (string id, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Id == id).FirstOrDefault();
    if (user == null)
    {
        return Results.NotFound(new { Message = "User not found" });
    }

    dbContext.Users.DeleteOne(u => u.Id == id);

    return Results.Ok(new { Message = "User deleted successfully" });
}).WithName("DeleteUser");

app.MapGet("/api/users/username/{username}", (string username, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Username == username).FirstOrDefault();
    if (user == null)
    {
        return Results.NotFound(new { Message = "User not found" });
    }

    return Results.Ok(user);
}).WithName("GetUserByUsername");

app.UseHealthChecks("/api/users/health");

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.Run();