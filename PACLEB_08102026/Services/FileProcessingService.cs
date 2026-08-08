using PACLEB_08102026.Models;
using System.Text.Json;

namespace PACLEB_08102026.Services;

public sealed class FileProcessingService : IFileProcessingService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<ProcessingResult> ProcessAsync(
        Stream stream,
        decimal minimumAmount,
        CancellationToken cancellationToken)
    {
        var records =
            await JsonSerializer.DeserializeAsync<List<InputRecord>>(
                stream,
                JsonOptions,
                cancellationToken)
            ?? throw new InvalidDataException(
                "The uploaded JSON file contains no records.");

        var filteredRecords = records
            .Where(x => x.Amount >= minimumAmount)
            .ToArray();

        return new ProcessingResult(
            records.Count,
            filteredRecords);
    }
}