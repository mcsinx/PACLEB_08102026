namespace PACLEB_08102026.Models;

public sealed record ProcessingResult(
    int TotalRecords,
    IReadOnlyCollection<InputRecord> Records);