using ClasificatorMaintenance.Database;
using ClasificatorMaintenance.Services.Ollama;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ClasificatorMaintenance.BackgroundServices;

internal sealed class IssuesPollingBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOllamaService ollamaService,
    ILogger<IssuesPollingBackgroundService> logger) : BackgroundService
{
    private IList<string> SupportedCategories;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SupportedCategories = await GetSupportedCategoriesAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

            await AnalyzeAndClassifyPendingIssuesAsync(stoppingToken);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
        }
    }

    private async Task AnalyzeAndClassifyPendingIssuesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ContextFlexiRooms>();

           var pendingIssues = await dbContext.Issues.AsNoTracking().ToListAsync();
         
            if (pendingIssues.Count == 0)
            {
                return;
            }

            foreach (var issue in pendingIssues)
            {
                try
                {
                    string category = await ClassifyIssueAsync(issue.Text, cancellationToken);
                    int issueTypeId = await GetOrCreateIssueTypeIdAsync(dbContext, category, cancellationToken);

                    await dbContext.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE \"Issues\" SET \"MaintenanceTypeId\" = {issueTypeId}, \"Status\" = {"classified"} WHERE \"Id\" = {issue.Id}",
                        cancellationToken);

                    logger.LogInformation(
                        "Issue {IssueId} classified as '{Category}' with IssueTypeId {IssueTypeId}.",
                        issue.Id,
                        category,
                        issueTypeId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Could not classify pending issue {IssueId}.", issue.Id);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while classifying pending issues.");
        }
    }

    private async Task<string> ClassifyIssueAsync(string issueText, CancellationToken cancellationToken)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string category in SupportedCategories)
        {
            stringBuilder.AppendLine($"- {category}");
        }

        string prompt = $$"""
            Eres un clasificador de tickets de mantenimiento hotelero.
            Clasifica el siguiente ticket en una sola categoría exacta de esta lista:

            {{stringBuilder.ToString()}}

            Responde únicamente con el nombre exacto de una categoría, sin explicación.

            Ticket:
            {{issueText}}
            """;

        string result = (await ollamaService.GenerateAsync(prompt, cancellationToken)).Trim();

        foreach (string category in SupportedCategories)
        {
            if (string.Equals(result, category, StringComparison.OrdinalIgnoreCase))
            {
                return category;
            }
        }

        throw new InvalidOperationException($"Invalid category returned by Ollama: '{result}'.");
    }

    private static async Task<int> GetOrCreateIssueTypeIdAsync(
        ContextFlexiRooms dbContext,
        string category,
        CancellationToken cancellationToken)
    {
        int? existingIssueTypeId = await dbContext.Database
            .SqlQuery<int?>(
                $"SELECT \"Id\" FROM \"MaintenanceTypes\" WHERE LOWER(\"Description\") = LOWER({category}) LIMIT 1")
            .SingleOrDefaultAsync(cancellationToken);

        if (existingIssueTypeId.HasValue)
        {
            return existingIssueTypeId.Value;
        }

        int newIssueTypeId = await dbContext.Database
            .SqlQuery<int>($"INSERT INTO \"MaintenanceTypes\" (\"Description\") VALUES ({category}) RETURNING \"Id\"")
            .SingleAsync(cancellationToken);

        return newIssueTypeId;
    }
    private async Task<IList<string>> GetSupportedCategoriesAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContextFlexiRooms>();
        return await dbContext.MaintenanceTypes
            .AsNoTracking()
            .OrderBy(x => x.Description)
            .Select(x => x.Description)
            .ToListAsync(cancellationToken);
    }
    private sealed record PendingIssueRow(int Id, string Texto);
}
