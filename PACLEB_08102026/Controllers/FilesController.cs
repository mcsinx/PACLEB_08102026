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
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                error = "The uploaded file is empty."
            });
        }

        if (file.Length > MaxFileSize)
        {
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
            return BadRequest(new
            {
                error = "Only JSON files are supported."
            });
        }

        var stopwatch = Stopwatch.StartNew();
        var fileName = Path.GetFileName(file.FileName);

        try
        {
            await using var stream = file.OpenReadStream();

            var result = await _processingService.ProcessAsync(
                stream,
                minimumAmount,
                cancellationToken);

            stopwatch.Stop();

            _trackingService.Record(
                new FileProcessingRecord(
                    fileName,
                    DateTimeOffset.UtcNow,
                    result.TotalRecords,
                    result.Records.Count,
                    stopwatch.ElapsedMilliseconds,
                    true));

            _logger.LogInformation(
                "Processed file {FileName}. Total records: {TotalRecords}. Matched records: {MatchedRecords}. Processing time: {ProcessingTime} ms.",
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
                ProcessingTimeMilliseconds = stopwatch.ElapsedMilliseconds
            });
        }
        catch (JsonException ex)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                ex,
                "Invalid JSON file uploaded: {FileName}",
                fileName);

            _trackingService.Record(
                new FileProcessingRecord(
                    fileName,
                    DateTimeOffset.UtcNow,
                    0,
                    0,
                    stopwatch.ElapsedMilliseconds,
                    false));

            return BadRequest(new
            {
                error = "The uploaded file contains invalid JSON."
            });
        }
        catch (InvalidDataException ex)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                ex,
                "Unable to process file: {FileName}",
                fileName);

            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }
}