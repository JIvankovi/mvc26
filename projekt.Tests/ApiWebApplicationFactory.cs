using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using projekt;
using projekt.Data;
using Xunit;

namespace projekt.Tests;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly object _databaseSync = new();
    private string? _adminConnectionString;
    private string? _testConnectionString;
    private string? _testDatabaseName;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            EnsureTemporaryDatabase(configBuilder.Build());

            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TestingConnection"] = _testConnectionString
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IConfigureOptions<AuthenticationOptions>));
            services.RemoveAll(typeof(IPostConfigureOptions<AuthenticationOptions>));

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme,
                    _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                options.DefaultScheme = TestAuthHandler.AuthenticationScheme;
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DropTemporaryDatabase();
        }

        base.Dispose(disposing);
    }

    private void EnsureTemporaryDatabase(IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(_testConnectionString))
        {
            return;
        }

        lock (_databaseSync)
        {
            if (!string.IsNullOrWhiteSpace(_testConnectionString))
            {
                return;
            }

            var baseConnectionString = configuration.GetConnectionString("TestingConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("A TestingConnection or DefaultConnection must be configured for integration tests.");

            var baseBuilder = new MySqlConnectionStringBuilder(baseConnectionString);
            var baseDatabase = string.IsNullOrWhiteSpace(baseBuilder.Database) ? "projekt_db" : baseBuilder.Database;
            var testDatabaseName = $"{baseDatabase}_it_{Guid.NewGuid():N}";

            var adminBuilder = new MySqlConnectionStringBuilder(baseBuilder.ConnectionString)
            {
                Database = string.Empty
            };

            using var connection = new MySqlConnection(adminBuilder.ConnectionString);
            connection.Open();

            using (var createCommand = connection.CreateCommand())
            {
                createCommand.CommandText = $"CREATE DATABASE `{EscapeIdentifier(testDatabaseName)}`;";
                createCommand.ExecuteNonQuery();
            }

            baseBuilder.Database = testDatabaseName;

            _adminConnectionString = adminBuilder.ConnectionString;
            _testConnectionString = baseBuilder.ConnectionString;
            _testDatabaseName = testDatabaseName;
        }
    }

    private void DropTemporaryDatabase()
    {
        if (string.IsNullOrWhiteSpace(_adminConnectionString) || string.IsNullOrWhiteSpace(_testDatabaseName))
        {
            return;
        }

        lock (_databaseSync)
        {
            if (string.IsNullOrWhiteSpace(_adminConnectionString) || string.IsNullOrWhiteSpace(_testDatabaseName))
            {
                return;
            }

            try
            {
                using var connection = new MySqlConnection(_adminConnectionString);
                connection.Open();

                using var dropCommand = connection.CreateCommand();
                dropCommand.CommandText = $"DROP DATABASE IF EXISTS `{EscapeIdentifier(_testDatabaseName)}`;";
                dropCommand.ExecuteNonQuery();
            }
            finally
            {
                _adminConnectionString = null;
                _testConnectionString = null;
                _testDatabaseName = null;
            }
        }
    }

    private static string EscapeIdentifier(string identifier)
    {
        return identifier.Replace("`", "``", StringComparison.Ordinal);
    }

}

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "TestScheme";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "admin@admin.com"),
            new(ClaimTypes.Email, "admin@admin.com"),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
