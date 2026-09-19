namespace ClasificatorMaintenance.Services.Ollama;

internal interface IOllamaService
{
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
}
