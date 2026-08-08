using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace PACLEB_08102026.Tests;

public class FilesApiTests
{
    private const string ApiKey =
        "integration-test-api-key";

    [Fact]
    public async Task ProcessFile_MissingApiKey_ReturnsUnauthorized()
    {
        // Arrange
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        using var content =
            CreateValidFileContent();

        // Act
        var response = await client.PostAsync(
            "/api/files/process?minimumAmount=1000",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ProcessFile_InvalidApiKey_ReturnsUnauthorized()
    {
        // Arrange
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-API-Key",
            "wrong-api-key");

        using var content =
            CreateValidFileContent();

        // Act
        var response = await client.PostAsync(
            "/api/files/process?minimumAmount=1000",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ProcessFile_ValidApiKeyAndJson_ReturnsOk()
    {
        // Arrange
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-API-Key",
            ApiKey);

        using var content =
            CreateValidFileContent();

        // Act
        var response = await client.PostAsync(
            "/api/files/process?minimumAmount=1000",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "\"totalRecords\":6",
            responseBody);

        Assert.Contains(
            "\"matchedRecords\":3",
            responseBody);
    }

    [Fact]
    public async Task ProcessFile_InvalidJson_ReturnsBadRequest()
    {
        // Arrange
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-API-Key",
            ApiKey);

        using var content =
            CreateInvalidFileContent();

        // Act
        var response = await client.PostAsync(
            "/api/files/process?minimumAmount=1000",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task ProcessFile_NegativeMinimumAmount_ReturnsBadRequest()
    {
        // Arrange
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-API-Key",
            ApiKey);

        using var content =
            CreateValidFileContent();

        // Act
        var response = await client.PostAsync(
            "/api/files/process?minimumAmount=-1",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Report_AfterSuccessfulProcessing_ReturnsOk()
    {
        // Arrange
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-API-Key",
            ApiKey);

        using var content =
            CreateValidFileContent();

        var processResponse =
            await client.PostAsync(
                "/api/files/process?minimumAmount=1000",
                content);

        Assert.Equal(
            HttpStatusCode.OK,
            processResponse.StatusCode);

        // Act
        var reportResponse =
            await client.GetAsync(
                "/api/files/report");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            reportResponse.StatusCode);

        var responseBody =
            await reportResponse.Content.ReadAsStringAsync();

        Assert.Contains(
            "\"processedFileCount\":1",
            responseBody);

        Assert.Contains(
            "orders.json",
            responseBody);
    }

    private static MultipartFormDataContent
        CreateValidFileContent()
    {
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

        return CreateMultipartContent(
            json,
            "orders.json");
    }

    private static MultipartFormDataContent
        CreateInvalidFileContent()
    {
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

        return CreateMultipartContent(
            json,
            "invalidOrders.json");
    }

    private static MultipartFormDataContent
        CreateMultipartContent(
            string json,
            string fileName)
    {
        var multipart =
            new MultipartFormDataContent();

        var fileContent =
            new ByteArrayContent(
                Encoding.UTF8.GetBytes(json));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(
                "application/json");

        multipart.Add(
            fileContent,
            "file",
            fileName);

        return multipart;
    }
}