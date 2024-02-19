using StatzAPI.Models;
using StatzAPI.Repositories;
using StatzAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StatzAPI", Version = "v1" });
});
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("ConnectionStrings:MongoDBConnection"));
builder.Services.AddSingleton<IMongoDBContext, MongoDBContext>();
builder.Services.AddTransient<IStatsRepository, StatsRepository>();
builder.Services.AddTransient<IStatsService, StatsService>();

var app = builder.Build();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StatsAPI V1");
    c.RoutePrefix = string.Empty;
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
