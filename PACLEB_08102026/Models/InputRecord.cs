namespace PACLEB_08102026.Models;

public sealed class InputRecord
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Amount { get; init; }
}