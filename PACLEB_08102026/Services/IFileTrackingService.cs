using PACLEB_08102026.Models;

namespace PACLEB_08102026.Services;

public interface IFileTrackingService
{
    void Record(FileProcessingRecord record);

    IReadOnlyCollection<FileProcessingRecord> GetRecords();

    int GetProcessedFileCount();
}