# PACLEB_08102026

.NET Core Coding Challenge - Secure JSON File Processing REST API

## Overview

This project is an ASP.NET Core Web API that securely processes uploaded JSON files.

The API filters records based on a specified minimum amount, tracks file-processing activity, and provides a reporting endpoint for processed files.

The solution demonstrates:

- ASP.NET Core REST API development
- API Key authentication using middleware
- JSON file processing and filtering
- Input validation
- Error handling and logging
- Thread-safe in-memory file tracking
- Dependency injection
- Unit testing
- API integration testing
- Swagger / OpenAPI
- Docker containerization

---

## Technologies

- .NET 8
- ASP.NET Core Web API
- C#
- System.Text.Json
- Swagger / OpenAPI
- xUnit
- Microsoft.AspNetCore.Mvc.Testing
- Docker

---

## Solution Structure

```text
PACLEB_08102026/
│
├── .dockerignore
├── PACLEB_08102026.sln
├── README.md
│
├── PACLEB_08102026/
│   ├── Controllers/
│   │   └── FilesController.cs
│   │
│   ├── Middleware/
│   │   └── ApiKeyMiddleware.cs
│   │
│   ├── Models/
│   │   ├── FileProcessingRecord.cs
│   │   ├── FileProcessingResponse.cs
│   │   ├── InputRecord.cs
│   │   └── ProcessingResult.cs
│   │
│   ├── Samples/
│   │   ├── orders.json
│   │   └── invalidOrders.json
│   │
│   ├── Services/
│   │   ├── FileProcessingService.cs
│   │   ├── FileTrackingService.cs
│   │   ├── IFileProcessingService.cs
│   │   └── IFileTrackingService.cs
│   │
│   ├── Dockerfile
│   ├── PACLEB_08102026.csproj
│   ├── PACLEB_08102026.http
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
└── PACLEB_08102026.Tests/
    ├── CustomWebApplicationFactory.cs
    ├── FileProcessingServiceTests.cs
    ├── FilesApiTests.cs
    └── FileTrackingServiceTests.cs
```

---

## API Authentication

The API uses API Key authentication implemented through ASP.NET Core middleware.

The API key must be included in the following HTTP request header:

```text
X-API-Key: <your-api-key>
```

### Local Development

For local development, the API key can be configured using Visual Studio User Secrets.

In Visual Studio 2022:

1. Right-click the `PACLEB_08102026` Web API project.
2. Select **Manage User Secrets**.
3. Add:

```json
{
  "ApiKey": "coding-challenge-key"
}
```

The API key is read from application configuration and is not hard-coded into the application source.

For Docker, the API key is supplied through an environment variable.

---

## API Endpoints

### Process JSON File

```http
POST /api/files/process?minimumAmount=1000
```

Uploads and processes a JSON file.

The endpoint accepts `multipart/form-data`.

The uploaded file field must be named:

```text
file
```

Required request header:

```text
X-API-Key: <your-api-key>
```

Example:

```text
POST /api/files/process?minimumAmount=1000
```

Using the included `orders.json` sample:

```text
Total records:   6
Matched records: 3
```

The matching records are:

```text
SO-2026-1001 - Maria Santos   1250.75
SO-2026-1004 - David Tan      2420.00
SO-2026-1005 - Sophia Lim     1599.95
```

### Processing Report

```http
GET /api/files/report
```

Returns file-processing information including:

- File name
- Processing timestamp
- Total record count
- Matching record count
- Processing duration
- Success/failure status

The reporting endpoint is also protected by API Key authentication.

---

## Sample JSON Files

A valid sample JSON file is included at:

```text
PACLEB_08102026/Samples/orders.json
```

Example:

```json
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
```

An intentionally malformed JSON file is also included for testing error handling:

```text
PACLEB_08102026/Samples/invalidOrders.json
```

---

## Prerequisites

To build and run the project locally:

- Visual Studio 2022
- .NET 8 SDK

To run the containerized application:

- Docker Desktop

---

## Building the Solution

Build commands should be executed from the **solution/repository root**:

```text
PACLEB_08102026/
```

Restore dependencies:

```powershell
dotnet restore
```

Build the solution:

```powershell
dotnet build
```

Alternatively:

```powershell
dotnet build PACLEB_08102026.sln
```

A successful build should complete with no errors.

---

## Running Automated Tests

The automated tests should be executed from the **solution/repository root**.

Run:

```powershell
dotnet test
```

Alternatively:

```powershell
dotnet test PACLEB_08102026.sln
```

The test suite includes unit and API integration tests covering:

- Valid JSON processing
- Filtering using the minimum amount
- Expected matching records
- Boundary conditions
- Invalid JSON
- Empty JSON collections
- File tracking
- Processing counters
- Missing API keys
- Invalid API keys
- Valid authenticated requests
- Invalid requests
- Reporting endpoint behavior

Tests can also be executed from Visual Studio 2022:

```text
Test -> Test Explorer -> Run All
```

All tests should pass before running or submitting the application.

---

## Running the API Locally

### Visual Studio 2022

1. Open:

```text
PACLEB_08102026.sln
```

2. Configure the API key using **Manage User Secrets**.

3. Set `PACLEB_08102026` as the startup project.

4. Select the normal HTTPS/project launch profile.

5. Press `F5`.

6. Swagger should open automatically.

7. Click **Authorize**.

8. Enter the configured API key:

```text
coding-challenge-key
```

9. Test the available endpoints.

### Command Line

From the solution root, move into the Web API project:

```powershell
cd .\PACLEB_08102026
```

Run the application:

```powershell
dotnet run
```

---

## Swagger / OpenAPI

Swagger is enabled for interactive API testing.

When running locally through Visual Studio, use the Swagger URL opened by the application.

It will normally look similar to:

```text
https://localhost:<port>/swagger
```

When running through Docker, Swagger is available at:

```text
http://localhost:8080/swagger
```

Click **Authorize** and provide the configured API key.

---

## Docker

The `Dockerfile` is located inside the main Web API project:

```text
PACLEB_08102026/PACLEB_08102026/Dockerfile
```

Docker commands are executed from the **Web API project directory**.

From the solution/repository root:

```powershell
cd .\PACLEB_08102026
```

### Build the Docker Image

Run:

```powershell
docker build --no-cache -t pacleb-file-api .
```

The resulting image will be named:

```text
pacleb-file-api
```

### Run the Docker Container

Run:

```powershell
docker run --rm -p 8080:8080 -e ApiKey="coding-challenge-key" pacleb-file-api
```

The application will be available at:

```text
http://localhost:8080
```

Swagger will be available at:

```text
http://localhost:8080/swagger
```

Click **Authorize** and enter:

```text
coding-challenge-key
```

---

## Example API Request

With the Docker container running, the processing endpoint can be called using curl.

Run the following command from the Web API project directory:

```powershell
curl.exe -X POST "http://localhost:8080/api/files/process?minimumAmount=1000" `
  -H "X-API-Key: coding-challenge-key" `
  -F "file=@Samples/orders.json;type=application/json"
```

Expected result:

```text
Total records:   6
Matched records: 3
```

The report endpoint can be called using:

```powershell
curl.exe -H "X-API-Key: coding-challenge-key" `
  http://localhost:8080/api/files/report
```

A request without the API key will return:

```text
401 Unauthorized
```

---

## File Processing

The application uses `System.Text.Json` to deserialize the uploaded file.

Records are filtered according to the supplied `minimumAmount`.

For example:

```text
minimumAmount = 1000
```

means only records whose amount is greater than or equal to `1000` are returned.

The file-processing logic is implemented in a dedicated service to keep processing responsibilities separate from the API controller.

---

## File Tracking

The application tracks each file-processing attempt.

Tracked information includes:

- File name
- Processing timestamp
- Total number of records
- Number of matching records
- Processing duration
- Success/failure status

The implementation uses:

```text
ConcurrentQueue<FileProcessingRecord>
```

to provide thread-safe processing history.

An atomic counter using:

```text
Interlocked
```

tracks successfully processed files.

The tracking service is registered as a singleton so processing history can be shared across multiple HTTP requests while the application is running.

### In-Memory Storage

For the scope of this coding challenge, file-processing history is stored in memory.

Therefore:

```text
Application restart -> Processing history is reset
```

For a production implementation, processing metadata would normally be stored in persistent storage.

---

## Validation and Error Handling

The API handles common failure scenarios including:

- Missing API key
- Invalid API key
- Missing file
- Empty file
- Unsupported file type
- Invalid JSON
- Empty JSON collection
- Negative minimum amount
- File exceeding the maximum allowed size
- Unexpected processing errors

Typical HTTP responses include:

```text
200 OK
400 Bad Request
401 Unauthorized
500 Internal Server Error
```

Internal exception details and sensitive information are not returned to API consumers.

---

## Logging

ASP.NET Core `ILogger` is used for application logging.

The application logs information related to:

- Start of file processing
- Successful file processing
- Validation failures
- Invalid JSON
- Authentication failures
- Unexpected processing errors

Sensitive information such as API keys is not written to the application logs.

---

## Design Decisions

### JSON Instead of CSV

JSON was selected because .NET provides native JSON support through:

```text
System.Text.Json
```

This keeps the solution simple and avoids introducing an unnecessary external dependency.

### API Key Middleware

API Key authentication is implemented using ASP.NET Core middleware.

This centralizes authentication logic and keeps security concerns separate from controller business logic.

### Service Layer

File processing and tracking are implemented through dedicated services and interfaces:

```text
IFileProcessingService
IFileTrackingService
```

This improves:

- Separation of concerns
- Maintainability
- Testability
- Dependency management

### Dependency Injection

ASP.NET Core dependency injection is used to provide processing and tracking services to the controller.

The processing service is registered separately from the singleton tracking service.

### Thread Safety

The tracking service needs to support concurrent HTTP requests.

The implementation therefore uses:

```text
ConcurrentQueue
Interlocked
```

instead of a regular static collection or counter.

### Asynchronous Processing

JSON deserialization uses asynchronous APIs.

The processing operation also supports:

```text
CancellationToken
```

allowing processing to stop if the associated HTTP request is cancelled.

---

## Limitations

The implementation intentionally keeps infrastructure simple and appropriate for the scope of the coding challenge.

Current limitations include:

- File-processing history is stored in memory.
- Tracking history is lost when the application restarts.
- Uploaded files are processed synchronously.
- Only JSON files are supported.
- Maximum supported file size is 5 MB.
- API Key authentication is intentionally simple.

---

## Possible Production Improvements

For an enterprise-scale implementation, the solution could be expanded with:

- Amazon S3 for uploaded file storage
- Amazon SQS for asynchronous processing jobs
- AWS Lambda for event-driven file processing
- ECS/Fargate for containerized background processing
- Persistent database storage for processing metadata
- AWS Secrets Manager or Parameter Store for secret management
- Amazon ElastiCache for distributed caching
- Elasticsearch/ELK for centralized logging and analysis
- OpenTelemetry for tracing and observability
- Health-check endpoints
- Rate limiting
- Malware scanning for uploaded files
- OAuth 2.0 / OpenID Connect authentication
- Configurable file-size and processing limits
- CI/CD pipelines for automated build, test, and deployment

---

## Author

Macky Pacleb