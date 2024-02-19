using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        services.AddHealthChecks();

        // Retrieve the environment variable indicating whether the app is in development mode
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Add CORS services
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
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

            return new MongoDbContext(Configuration);
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
        });
        services.AddHealthChecks();

        // Add logging services
        services.AddLogging(loggingBuilder =>
        {
            // Configure console logging
            loggingBuilder.AddConsole();
        });
    }
}