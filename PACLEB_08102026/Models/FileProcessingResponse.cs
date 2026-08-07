namespace PACLEB_08102026.Models;

public sealed class FileProcessingResponse
{
    public required string FileName { get; init; }

    public int TotalRecords { get; init; }

    public int MatchedRecords { get; init; }

    public decimal MinimumAmount { get; init; }

    public required IReadOnlyCollection<InputRecord> Records { get; init; }

    public long ProcessingTimeMilliseconds { get; init; }
}