using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PACLEB_08102026.Models;
using PACLEB_08102026.Services;

namespace PACLEB_08102026.Controllers;

[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB max file

    private readonly IFileProcessingService _processingService;
    private readonly IFileTrackingService _trackingService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(
        IFileProcessingService processingService,
        IFileTrackingService trackingService,
        ILogger<FilesController> logger)
    {
        _processingService = processingService;
        _trackingService = trackingService;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<ActionResult<FileProcessingResponse>> ProcessFile(
     IFormFile file,
     [FromQuery] decimal minimumAmount = 0,
     CancellationToken cancellationToken = default)
    {
        if (file == null)
        {
            _logger.LogWarning("File processing request received without a file.");

            return BadRequest(new
            {
                error = "A file is required."
            });
        }

        if (file.Length == 0)
        {
            _logger.LogWarning(
                "Empty file uploaded: {FileName}",
                file.FileName);

            return BadRequest(new
            {
                error = "The uploaded file is empty."
            });
        }

        if (file.Length > MaxFileSize)
        {
            _logger.LogWarning(
                "File {FileName} exceeded maximum size. Size: {FileSize} bytes.",
                file.FileName,
                file.Length);

            return BadRequest(new
            {
                error = "The maximum supported file size is 5 MB."
            });
        }

        if (!string.Equals(
                Path.GetExtension(file.FileName),
                ".json",
                StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Unsupported file type uploaded: {FileName}",
                file.FileName);

            return BadRequest(new
            {
                error = "Only JSON files are supported."
            });
        }

        if (minimumAmount < 0)
        {
            return BadRequest(new
            {
                error = "Minimum amount cannot be negative."
            });
        }

        var stopwatch = Stopwatch.StartNew();
        var fileName = Path.GetFileName(file.FileName);

        try
        {
            _logger.LogInformation(
                "Starting processing for file {FileName}.",
                fileName);

            await using var stream = file.OpenReadStream();

            var result = await _processingService.ProcessAsync(
                stream,
                minimumAmount,
                cancellationToken);

            stopwatch.Stop();

            var processingRecord = new FileProcessingRecord(
                fileName,
                DateTimeOffset.UtcNow,
                result.TotalRecords,
                result.Records.Count,
                stopwatch.ElapsedMilliseconds,
                true);

            _trackingService.Record(processingRecord);

            _logger.LogInformation(
                "Successfully processed file {FileName}. " +
                "Total records: {TotalRecords}. " +
                "Matched records: {MatchedRecords}. " +
                "Processing time: {ProcessingTime} ms.",
                fileName,
                result.TotalRecords,
                result.Records.Count,
                stopwatch.ElapsedMilliseconds);

            return Ok(new FileProcessingResponse
            {
                FileName = fileName,
                TotalRecords = result.TotalRecords,
                MatchedRecords = result.Records.Count,
                MinimumAmount = minimumAmount,
                Records = result.Records,
                ProcessingTimeMilliseconds =
                    stopwatch.ElapsedMilliseconds
            });
        }
        catch (JsonException ex)
        {
            stopwatch.Stop();

            TrackFailure(
                fileName,
                stopwatch.ElapsedMilliseconds);

            _logger.LogWarning(
                ex,
                "Invalid JSON content in file {FileName}.",
                fileName);

            return BadRequest(new
            {
                error = "The uploaded file contains invalid JSON."
            });
        }
        catch (InvalidDataException ex)
        {
            stopwatch.Stop();

            TrackFailure(
                fileName,
                stopwatch.ElapsedMilliseconds);

            _logger.LogWarning(
                ex,
                "Invalid data encountered while processing {FileName}.",
                fileName);

            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Processing was cancelled for file {FileName}.",
                fileName);

            throw;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            TrackFailure(
                fileName,
                stopwatch.ElapsedMilliseconds);

            _logger.LogError(
                ex,
                "Unexpected error while processing file {FileName}.",
                fileName);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "An unexpected error occurred while processing the file."
                });
        }
    }


    [HttpGet("report")]
    public IActionResult GetReport()
    {
        var records = _trackingService
            .GetRecords()
            .OrderByDescending(x => x.ProcessedAt)
            .ToArray();

        return Ok(new
        {
            processedFileCount = _trackingService.GetProcessedFileCount(),
            files = records
        });
    }

    private void TrackFailure(
    string fileName,
    long processingTimeMilliseconds)
    {
        _trackingService.Record(
            new FileProcessingRecord(
                fileName,
                DateTimeOffset.UtcNow,
                0,
                0,
                processingTimeMilliseconds,
                false));
    }
}