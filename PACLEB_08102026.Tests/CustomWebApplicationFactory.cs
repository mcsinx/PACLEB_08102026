using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace PACLEB_08102026.Tests;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                var testSettings =
                    new Dictionary<string, string?>
                    {
                        ["ApiKey"] = "integration-test-api-key"
                    };

                configuration.AddInMemoryCollection(testSettings);
            });
    }
}