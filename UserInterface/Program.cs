using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

Process npmProcess = null;

    var clientAppPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp");
    if (Directory.Exists(clientAppPath))
    {
        var npmStart = new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = "/c npm start",
            WorkingDirectory = clientAppPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        npmProcess = Process.Start(npmStart);
        npmProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
        npmProcess.BeginOutputReadLine();
        npmProcess.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
        npmProcess.BeginErrorReadLine();

        app.Lifetime.ApplicationStopping.Register(() =>
        {
            if (npmProcess != null && !npmProcess.HasExited)
            {
                npmProcess.Kill();
                npmProcess.Dispose();
            }
        });
    }
    else
    {
        Console.WriteLine($"ClientApp directory not found: {clientAppPath}");
    }

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();