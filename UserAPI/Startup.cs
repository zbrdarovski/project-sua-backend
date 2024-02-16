// Startup.cs

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"] ?? string.Empty);

        // Add the HTTP client factory
        services.AddHttpClient();

        services.AddHttpContextAccessor();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };
            });

        services.AddAuthorization();

        services.AddMvc()
            .AddSessionStateTempDataProvider();

        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(1800);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddHealthChecks();

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
        services.AddCors(options =>
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

        services.AddSingleton<MongoDbContext>(sp =>
        {
            var mongoDbSettings = Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

            if (mongoDbSettings == null)
            {
                throw new ArgumentNullException(nameof(mongoDbSettings), "MongoDB settings are missing in configuration.");
            }

            if (mongoDbSettings.ConnectionString is null)
            {
                throw new ArgumentNullException(nameof(mongoDbSettings.ConnectionString), "MongoDB connection string is missing in configuration.");
            }

            if (mongoDbSettings.DatabaseName is null)
            {
                throw new ArgumentNullException(nameof(mongoDbSettings.DatabaseName), "MongoDB database name is missing in configuration.");
            }

            return new MongoDbContext(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName);
        });

        // Add logging services
        services.AddLogging(loggingBuilder =>
        {
            // Configure console logging
            loggingBuilder.AddConsole();
        });
    }
}