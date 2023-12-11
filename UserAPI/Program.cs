// Program.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

var salt = builder.Configuration["Salt"];

builder.Services.AddEndpointsApiExplorer();
// Inside ConfigureServices method
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Uporabniska avtentikacija API", Version = "v1" });

    // Define the Swagger security scheme for JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Define the Swagger security requirement for JWT
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
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
                   .AllowAnyMethod()
                   .AllowCredentials();
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

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Uporabniska avtentikacija API"));
}

// Ensure UseRouting is called before UseAuthorization
app.UseRouting();

// UseCors should be placed after UseRouting and before UseAuthorization
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication(); // Must be after UseRouting()
app.UseAuthorization(); // Must be after UseAuthentication()

app.UseEndpoints(endpoints =>
{
    // ... endpoint configuration
});

app.MapPost("/api/users/register", (UserRegistrationDto userDto, [FromServices] MongoDbContext dbContext) =>
{
    // Combine the password and salt, then hash the result
    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password + salt);

    var user = new User { Id = userDto.Id, Username = userDto.Username, Password = hashedPassword };
    dbContext.Users.InsertOne(user);

    return Results.Ok(new { Message = "Registration successful" });
}).WithName("Register");

string GenerateJwtToken(string userId, string key, string issuer)
{
    if (userId is null || key is null || issuer is null)
    {
        // Handle the null reference gracefully or throw an exception.
        throw new ArgumentNullException("userId, key, and issuer cannot be null.");
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenKey = Encoding.ASCII.GetBytes(key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userId)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
        Issuer = issuer
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.MapPost("/api/users/login", (UserLoginDto userDto, [FromServices] MongoDbContext dbContext, [FromServices] IConfiguration configuration) =>
{
    var user = dbContext.Users.Find(u => u.Username == userDto.Username).FirstOrDefault();

    if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password + salt, user.Password))
    {
        return Results.Unauthorized();
    }

    // Generate JWT token
    var token = GenerateJwtToken(user.Id, configuration["Jwt:Key"], configuration["Jwt:Issuer"]);

    // Return token and user ID in response
    return Results.Ok(new { Message = "Connection successful", Token = token, UserId = user.Id });
}).WithName("Login");


app.MapPost("/api/users/change-password", (ChangePasswordDto changePasswordDto, [FromServices] MongoDbContext dbContext, [FromServices] IHttpContextAccessor httpContextAccessor) =>
{
    var user = dbContext.Users.Find(u => u.Username == changePasswordDto.Username).FirstOrDefault();

    if (user == null || !BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword + salt, user.Password))
    {
        return Results.Unauthorized();
    }

    // Check for JWT token in the request headers
    var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
    if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.ToString().StartsWith("Bearer "))
    {
        return Results.Unauthorized();
    }

    var token = authorizationHeader.ToString().Substring("Bearer ".Length);
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(app.Configuration["Jwt:Key"]);

    // Validate JWT token
    try
    {
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        }, out var validatedToken);
    }
    catch (Exception)
    {
        // Token validation failed
        return Results.Unauthorized();
    }

    // Combine the new password and salt, then hash the result
    string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword + salt);

    user.Password = hashedNewPassword;

    dbContext.Users.ReplaceOne(u => u.Id == user.Id, user);

    return Results.Ok(new { Message = "Password changed successfully" });
}).WithName("ChangePassword").RequireAuthorization();

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
}).WithName("UpdateUser").RequireAuthorization();

app.MapDelete("/api/users/{id}", (string id, [FromServices] MongoDbContext dbContext) =>
{
    var user = dbContext.Users.Find(u => u.Id == id).FirstOrDefault();
    if (user == null)
    {
        return Results.NotFound(new { Message = "User not found" });
    }

    dbContext.Users.DeleteOne(u => u.Id == id);

    return Results.Ok(new { Message = "User deleted successfully" });
}).WithName("DeleteUser").RequireAuthorization();

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

app.Run();