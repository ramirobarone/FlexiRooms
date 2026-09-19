using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ClasificatorMaintenance.Services.Ollama;

internal sealed class OllamaService(
    HttpClient httpClient,
    IOptions<OllamaOptions> options) : IOllamaService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly OllamaOptions _options = options.Value;

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt is required.", nameof(prompt));
        }

        if (string.IsNullOrWhiteSpace(_options.Model))
        {
            throw new InvalidOperationException("Ollama model configuration is required.");
        }

        var request = new OllamaGenerateRequest(_options.Model, prompt, false);

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/generate", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        OllamaGenerateResponse? content = await response.Content
            .ReadFromJsonAsync<OllamaGenerateResponse>(JsonOptions, cancellationToken);

        if (string.IsNullOrWhiteSpace(content?.Response))
        {
            throw new InvalidOperationException("Ollama response content is empty.");
        }

        return content.Response;
    }

    private sealed record OllamaGenerateRequest(string Model, string Prompt, bool Stream);

    private sealed record OllamaGenerateResponse(string Response);
}
