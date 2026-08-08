using System.Text.Json;
using PACLEB_08102026.Models;

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
        ArgumentNullException.ThrowIfNull(stream);

        var records =
            await JsonSerializer.DeserializeAsync<List<InputRecord>>(
                stream,
                JsonOptions,
                cancellationToken);

        if (records is null || records.Count == 0)
        {
            throw new InvalidDataException(
                "The uploaded JSON file does not contain any records.");
        }

        var filteredRecords = records
            .Where(record => record.Amount >= minimumAmount)
            .ToArray();

        return new ProcessingResult(
            records.Count,
            filteredRecords);
    }
}