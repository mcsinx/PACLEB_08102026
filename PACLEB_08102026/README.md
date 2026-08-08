# PACLEB_08102026

.NET Core Coding Challenge - Secure JSON File Processing REST API

## Overview

This project is an ASP.NET Core Web API that securely processes uploaded JSON files.

The API filters records based on a minimum amount, tracks processed files, and provides a reporting endpoint.

The project demonstrates:

- ASP.NET Core REST API development
- API Key authentication
- JSON file processing
- Input validation
- Error handling and logging
- Thread-safe in-memory file tracking
- Unit and integration testing
- Docker containerization

## Technologies

- .NET 8
- ASP.NET Core Web API
- C#
- System.Text.Json
- Swagger / OpenAPI
- xUnit
- Docker

## API Authentication

The API uses API Key authentication.

Send the API key using the following request header:

    X-API-Key: coding-challenge-key

For local development, the API key can be configured using Visual Studio User Secrets.

Right-click the project and select:

    Manage User Secrets

Then add:

    {
      "ApiKey": "coding-challenge-key"
    }

For Docker, provide the key through an environment variable.

## API Endpoints

### Process File

    POST /api/files/process?minimumAmount=1000

Uploads and processes a JSON file.

The request must use `multipart/form-data` with a field named:

    file

Example sample file:

    Samples/orders.json

With `minimumAmount=1000`, the provided sample contains:

- 6 total records
- 3 matching records

### Processing Report

    GET /api/files/report

Returns information about processed files including:

- File name
- Processing date/time
- Total record count
- Matching record count
- Processing duration
- Success/failure status

## Sample JSON

A sample file is provided at:

    Samples/orders.json

Example:

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

An intentionally invalid JSON file is also provided for error-handling tests:

    Samples/invalidOrders.json

## Running Locally

### Prerequisites

- Visual Studio 2022
- .NET 8 SDK

### Visual Studio

1. Open `PACLEB_08102026.sln`.
2. Configure the API key using User Secrets.
3. Build the solution.
4. Run the Web API.
5. Open Swagger.
6. Click `Authorize`.
7. Enter the configured API key.
8. Test the available endpoints.

### Command Line

From the project directory:

    dotnet restore
    dotnet build
    dotnet run

## Testing

The solution contains automated tests using xUnit.

The tests cover:

- Valid JSON processing
- Filtering by minimum amount
- Invalid JSON
- Empty input
- Boundary conditions
- File tracking
- Processing counters
- Missing API key
- Invalid API key
- Successful API requests
- Reporting endpoint

Run the tests using Visual Studio Test Explorer or:

    dotnet test

## Docker

Build the Docker image from the project directory:

    docker build -t pacleb-file-api .

Run the container:

    docker run --rm -p 8080:8080 -e ApiKey="coding-challenge-key" pacleb-file-api

Then open:

    http://localhost:8080/swagger

Click `Authorize` and enter:

    coding-challenge-key

## File Tracking

The application tracks file processing information using an in-memory tracking service.

The implementation uses:

- `ConcurrentQueue` for thread-safe processing history
- `Interlocked` for thread-safe counter updates

Tracking information is shared between requests while the application is running.

Because this coding challenge uses in-memory storage, tracking information is reset when the application restarts.

In a production environment, processing history would normally be stored in persistent storage.

## Error Handling

The API handles common error scenarios including:

- Missing API key
- Invalid API key
- Empty files
- Unsupported file types
- Invalid JSON
- Empty JSON collections
- Negative minimum amounts
- Oversized files
- Unexpected processing failures

The service returns appropriate HTTP status codes such as:

- `200 OK`
- `400 Bad Request`
- `401 Unauthorized`
- `500 Internal Server Error`

## Logging

ASP.NET Core `ILogger` is used for application logging.

Logging covers:

- Start of file processing
- Successful processing
- Validation errors
- Invalid JSON
- Authentication failures
- Unexpected errors

Sensitive information such as API keys is not logged.

## Design Decisions

### JSON File Processing

JSON was selected because .NET provides native support through `System.Text.Json`, avoiding unnecessary external dependencies.

### API Key Middleware

Authentication is implemented using middleware so security validation is centralized and separated from controller business logic.

### Service Layer

File processing and file tracking are implemented as separate services to improve separation of concerns, maintainability, and testability.

### Thread Safety

The tracking service is registered as a singleton because the processing history needs to be shared across requests.

`ConcurrentQueue` and `Interlocked` are used to support concurrent requests safely.

## Limitations

For the scope of this coding challenge:

- File tracking is stored in memory.
- Tracking history is lost when the application restarts.
- Files are processed synchronously.
- Only JSON files are supported.
- Maximum file size is 5 MB.
- API Key authentication is intentionally simple.

## Possible Production Improvements

For a production environment, the solution could be expanded with:

- Amazon S3 for uploaded file storage
- Amazon SQS for asynchronous file processing
- AWS Lambda or ECS/Fargate for background processing
- Persistent database storage for processing history
- AWS Secrets Manager for secret management
- Amazon ElastiCache for caching
- Elasticsearch/ELK for centralized logging
- OpenTelemetry for distributed tracing
- Health checks
- Rate limiting
- OAuth 2.0 / OpenID Connect authentication
- CI/CD pipelines

## Author

Macky Pacleb