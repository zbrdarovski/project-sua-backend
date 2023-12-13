﻿// Startup.cs

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

        // Configure CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("http://localhost:44459") // Add your frontend URL here
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

            return new MongoDbContext(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName);
        });
    }
}