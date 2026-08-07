namespace PACLEB_08102026.Models;

public sealed record FileProcessingRecord(
    string FileName,
    DateTimeOffset ProcessedAt,
    int TotalRecords,
    int MatchedRecords,
    long ProcessingTimeMilliseconds,
    bool Successful);