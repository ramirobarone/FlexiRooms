namespace ClasificatorMaintenance.Services.Ollama;

internal sealed class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string? BaseUrl { get; set; }

    public string? Model { get; set; }
}
