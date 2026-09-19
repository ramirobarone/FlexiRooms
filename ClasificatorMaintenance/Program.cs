using ClasificatorMaintenance.BackgroundServices;
using ClasificatorMaintenance.Database;
using ClasificatorMaintenance.Services.Ollama;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

string? connectionString = builder.Configuration["ConnectionStrings:hotelis"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'hotelis' is required.");
}

builder.Services.AddDbContext<ContextFlexiRooms>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.AddHttpClient<IOllamaService, OllamaService>((serviceProvider, httpClient) =>
{
    OllamaOptions ollamaOptions = serviceProvider.GetRequiredService<IOptions<OllamaOptions>>().Value;

    if (string.IsNullOrWhiteSpace(ollamaOptions.BaseUrl))
    {
        throw new InvalidOperationException("Ollama base URL configuration is required.");
    }

    httpClient.BaseAddress = new Uri(ollamaOptions.BaseUrl, UriKind.Absolute);
});

builder.Services.AddHostedService<IssuesPollingBackgroundService>();

using IHost host = builder.Build();
await host.RunAsync();

