using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using projekt.Data;
using Xunit;

namespace projekt.Tests;

[Collection("Api integration")]
public class FilesIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public FilesIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Upload_ValidFile_ReturnsOk_AndAppearsInList()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var pageResponse = await client.GetAsync("/Files");
        pageResponse.EnsureSuccessStatusCode();
        var pageHtml = await pageResponse.Content.ReadAsStringAsync();
        var antiforgeryToken = ExtractAntiforgeryToken(pageHtml);

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(antiforgeryToken), "__RequestVerificationToken");

        var bytes = Encoding.UTF8.GetBytes("integration-test-file");
        using var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        formData.Add(fileContent, "file", "upload-test.txt");

        var uploadResponse = await client.PostAsync("/Files/Upload", formData);

        var uploadBody = await uploadResponse.Content.ReadAsStringAsync();
        Assert.True(uploadResponse.StatusCode != HttpStatusCode.InternalServerError, uploadBody);
        Assert.Equal(HttpStatusCode.OK, uploadResponse.StatusCode);

        var listResponse = await client.GetAsync("/Files/List");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var files = await listResponse.Content.ReadFromJsonAsync<List<FileListItem>>();
        Assert.NotNull(files);
        Assert.Contains(files!, file => file.OriginalFileName == "upload-test.txt");

        await CleanupUploadedFilesAsync();
    }

    private async Task CleanupUploadedFilesAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var uploadedFiles = dbContext.UploadedFiles.ToList();

        dbContext.UploadedFiles.RemoveRange(uploadedFiles);
        await dbContext.SaveChangesAsync();
    }

    private sealed class FileListItem
    {
        public string OriginalFileName { get; set; } = string.Empty;
    }

    private static string ExtractAntiforgeryToken(string html)
    {
        var match = Regex.Match(
            html,
            "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(?<token>[^\"]+)\"",
            RegexOptions.IgnoreCase);

        Assert.True(match.Success, "Antiforgery token was not found in the Files page HTML.");
        return match.Groups["token"].Value;
    }
}