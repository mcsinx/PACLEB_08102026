using PACLEB_08102026.Models;
using PACLEB_08102026.Services;
using System.Collections.Concurrent;

namespace PACLEB_08102026.Services;

public sealed class FileTrackingService : IFileTrackingService
{
    private readonly ConcurrentQueue<FileProcessingRecord> _records = new();

    private int _processedFileCount;

    public void Record(FileProcessingRecord record)
    {
        _records.Enqueue(record);

        if (record.Successful)
        {
            Interlocked.Increment(ref _processedFileCount);
        }
    }

    public IReadOnlyCollection<FileProcessingRecord> GetRecords()
    {
        return _records.ToArray();
    }

    public int GetProcessedFileCount()
    {
        return Volatile.Read(ref _processedFileCount);
    }
}