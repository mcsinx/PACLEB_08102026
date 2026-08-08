using PACLEB_08102026.Models;
using PACLEB_08102026.Services;

namespace PACLEB_08102026.Tests;

public class FileTrackingServiceTests
{
    [Fact]
    public void Record_SuccessfulFile_IncrementsProcessedFileCount()
    {
        // Arrange
        var service = new FileTrackingService();

        var record = new FileProcessingRecord(
            "orders.json",
            DateTimeOffset.UtcNow,
            5,
            3,
            10,
            true);

        // Act
        service.Record(record);

        // Assert
        Assert.Equal(1, service.GetProcessedFileCount());
    }

    [Fact]
    public void Record_FailedFile_DoesNotIncrementProcessedFileCount()
    {
        // Arrange
        var service = new FileTrackingService();

        var record = new FileProcessingRecord(
            "invalid.json",
            DateTimeOffset.UtcNow,
            0,
            0,
            5,
            false);

        // Act
        service.Record(record);

        // Assert
        Assert.Equal(0, service.GetProcessedFileCount());
    }

    [Fact]
    public void Record_AddsProcessingRecordToHistory()
    {
        // Arrange
        var service = new FileTrackingService();

        var record = new FileProcessingRecord(
            "orders.json",
            DateTimeOffset.UtcNow,
            5,
            3,
            10,
            true);

        // Act
        service.Record(record);

        var records = service.GetRecords();

        // Assert
        Assert.Single(records);

        var storedRecord = records.Single();

        Assert.Equal("orders.json", storedRecord.FileName);
        Assert.Equal(5, storedRecord.TotalRecords);
        Assert.Equal(3, storedRecord.MatchedRecords);
        Assert.True(storedRecord.Successful);
    }

    [Fact]
    public void GetProcessedFileCount_MultipleSuccessfulFiles_ReturnsCorrectCount()
    {
        // Arrange
        var service = new FileTrackingService();

        service.Record(CreateRecord("file1.json", true));
        service.Record(CreateRecord("file2.json", true));
        service.Record(CreateRecord("invalid.json", false));

        // Act
        var count = service.GetProcessedFileCount();

        // Assert
        Assert.Equal(2, count);
    }

    private static FileProcessingRecord CreateRecord(
        string fileName,
        bool successful)
    {
        return new FileProcessingRecord(
            fileName,
            DateTimeOffset.UtcNow,
            successful ? 5 : 0,
            successful ? 3 : 0,
            10,
            successful);
    }
}