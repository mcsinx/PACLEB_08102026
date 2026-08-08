using System.Text;
using System.Text.Json;
using PACLEB_08102026.Services;

namespace PACLEB_08102026.Tests;

public class FileProcessingServiceTests
{
    private readonly FileProcessingService _service = new();

    [Fact]
    public async Task ProcessAsync_ValidJson_ReturnsRecordsMatchingMinimumAmount()
    {
        // Arrange
        const string json = """
        [
          {
            "id": 1001,
            "name": "SO-2026-1001 - Maria Santos",
            "amount": 1250.75
          },
          {
            "id": 1002,
            "name": "SO-2026-1002 - John Reyes",
            "amount": 349.99
          },
          {
            "id": 1003,
            "name": "SO-2026-1003 - Angela Cruz",
            "amount": 875.50
          },
          {
            "id": 1004,
            "name": "SO-2026-1004 - David Tan",
            "amount": 2420.00
          },
          {
            "id": 1005,
            "name": "SO-2026-1005 - Sophia Lim",
            "amount": 1599.95
          },
          {
            "id": 1006,
            "name": "SO-2026-1006 - Carlo Mendoza",
            "amount": 499.00
          }
        ]
        """;

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _service.ProcessAsync(
            stream,
            1000,
            CancellationToken.None);

        // Assert
        Assert.Equal(6, result.TotalRecords);
        Assert.Equal(3, result.Records.Count);

        Assert.All(
            result.Records,
            record => Assert.True(record.Amount >= 1000));
    }

    [Fact]
    public async Task ProcessAsync_ValidJson_ReturnsExpectedMatchingOrders()
    {
        // Arrange
        const string json = """
        [
          {
            "id": 1001,
            "name": "SO-2026-1001 - Maria Santos",
            "amount": 1250.75
          },
          {
            "id": 1002,
            "name": "SO-2026-1002 - John Reyes",
            "amount": 349.99
          },
          {
            "id": 1003,
            "name": "SO-2026-1003 - Angela Cruz",
            "amount": 875.50
          },
          {
            "id": 1004,
            "name": "SO-2026-1004 - David Tan",
            "amount": 2420.00
          },
          {
            "id": 1005,
            "name": "SO-2026-1005 - Sophia Lim",
            "amount": 1599.95
          },
          {
            "id": 1006,
            "name": "SO-2026-1006 - Carlo Mendoza",
            "amount": 499.00
          }
        ]
        """;

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _service.ProcessAsync(
            stream,
            1000,
            CancellationToken.None);

        var records = result.Records
            .OrderBy(record => record.Id)
            .ToArray();

        // Assert
        Assert.Equal(3, records.Length);

        Assert.Equal(1001, records[0].Id);
        Assert.Equal("SO-2026-1001 - Maria Santos", records[0].Name);
        Assert.Equal(1250.75m, records[0].Amount);

        Assert.Equal(1004, records[1].Id);
        Assert.Equal("SO-2026-1004 - David Tan", records[1].Name);
        Assert.Equal(2420.00m, records[1].Amount);

        Assert.Equal(1005, records[2].Id);
        Assert.Equal("SO-2026-1005 - Sophia Lim", records[2].Name);
        Assert.Equal(1599.95m, records[2].Amount);
    }

    [Fact]
    public async Task ProcessAsync_NoMatchingRecords_ReturnsEmptyCollection()
    {
        // Arrange
        const string json = """
        [
          {
            "id": 1001,
            "name": "SO-2026-1001 - Maria Santos",
            "amount": 1250.75
          },
          {
            "id": 1002,
            "name": "SO-2026-1002 - John Reyes",
            "amount": 349.99
          }
        ]
        """;

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _service.ProcessAsync(
            stream,
            5000,
            CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalRecords);
        Assert.Empty(result.Records);
    }

    [Fact]
    public async Task ProcessAsync_EmptyArray_ThrowsInvalidDataException()
    {
        // Arrange
        const string json = "[]";

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _service.ProcessAsync(
                stream,
                1000,
                CancellationToken.None));
    }

    [Fact]
    public async Task ProcessAsync_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        const string json = """
        [
          {
            "id": 2001,
            "name": "SO-2026-2001 - Roberto Garcia",
            "amount": 1899.50
          },
          {
            "id": 2002,
            "name": "SO-2026-2002 - Patricia Gomez",
            "amount": 725.25
        """;

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() =>
            _service.ProcessAsync(
                stream,
                1000,
                CancellationToken.None));
    }

    [Fact]
    public async Task ProcessAsync_MinimumAmountEqualToOrderAmount_IncludesOrder()
    {
        // Arrange
        const string json = """
        [
          {
            "id": 1001,
            "name": "SO-2026-1001 - Maria Santos",
            "amount": 1250.75
          },
          {
            "id": 1002,
            "name": "SO-2026-1002 - John Reyes",
            "amount": 349.99
          }
        ]
        """;

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var result = await _service.ProcessAsync(
            stream,
            1250.75m,
            CancellationToken.None);

        // Assert
        Assert.Single(result.Records);

        var record = result.Records.Single();

        Assert.Equal(1001, record.Id);
        Assert.Equal(1250.75m, record.Amount);
    }

    [Fact]
    public async Task ProcessAsync_NullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ProcessAsync(
                null!,
                1000,
                CancellationToken.None));
    }
}