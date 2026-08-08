using PACLEB_08102026.Models;

namespace PACLEB_08102026.Services;

public interface IFileProcessingService
{
    Task<ProcessingResult> ProcessAsync(
        Stream stream,
        decimal minimumAmount,
        CancellationToken cancellationToken);
}