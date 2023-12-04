// Startup.cs

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();

        // Configure CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("https://localhost:44459/") // Add your frontend URL here
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