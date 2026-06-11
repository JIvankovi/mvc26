using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using projekt.DTOs;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;
using Xunit;

namespace projekt.Tests;

[Collection("Api integration")]
public class DeviceApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private const int MissingId = int.MaxValue;

    private readonly ApiWebApplicationFactory _factory;

    public DeviceApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_CreateDevice_ReturnsCreated()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/devices/api", new DeviceFormViewModel
        {
            Name = "Integration Scope",
            Manufacturer = "Acme",
            SerialNumber = "INT-001",
            PurchaseDate = new DateTime(2026, 6, 11),
            MeasurementType = MeasurementType.Voltage
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<DeviceDto>();
        Assert.NotNull(created);
        Assert.Equal("Integration Scope", created!.Name);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var createdEntity = await dbContext.Devices.FindAsync(created.Id);
        if (createdEntity != null)
        {
            dbContext.Devices.Remove(createdEntity);
            await dbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Get_ListDevices_ReturnsOk()
    {
        var deviceId = await SeedDeviceAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/devices/api");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var devices = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();
        Assert.NotNull(devices);
        Assert.Contains(devices!, device => device.Id == deviceId);

        await DeleteDeviceAsync(deviceId);
    }

    [Fact]
    public async Task Post_InvalidDevice_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/devices/api", new DeviceFormViewModel
        {
            Name = string.Empty,
            Manufacturer = "Acme",
            SerialNumber = "INT-002",
            PurchaseDate = new DateTime(2026, 6, 11),
            MeasurementType = MeasurementType.Voltage
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdateExistingDevice_ReturnsOk()
    {
        var deviceId = await SeedDeviceAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/devices/api/{deviceId}", new DeviceFormViewModel
        {
            Id = deviceId,
            Name = "Updated Device",
            Manufacturer = "Updated Manufacturer",
            SerialNumber = "UPD-100",
            PurchaseDate = new DateTime(2026, 1, 15),
            MeasurementType = MeasurementType.Temperature
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var saved = await dbContext.Devices.FindAsync(deviceId);
        Assert.NotNull(saved);
        Assert.Equal("Updated Device", saved!.Name);
        Assert.Equal(MeasurementType.Temperature, saved.MeasurementType);

        await DeleteDeviceAsync(deviceId);
    }

    [Fact]
    public async Task Put_InvalidDevice_ReturnsBadRequest()
    {
        var deviceId = await SeedDeviceAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/devices/api/{deviceId}", new DeviceFormViewModel
        {
            Id = deviceId,
            Name = string.Empty,
            Manufacturer = "Updated Manufacturer",
            SerialNumber = "UPD-100",
            PurchaseDate = new DateTime(2026, 1, 15),
            MeasurementType = MeasurementType.Temperature
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await DeleteDeviceAsync(deviceId);
    }

    [Fact]
    public async Task Put_NonExistentDevice_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/devices/api/{MissingId}", new DeviceFormViewModel
        {
            Id = MissingId,
            Name = "Missing Device",
            MeasurementType = MeasurementType.Voltage
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingDevice_ReturnsNoContent()
    {
        var deviceId = await SeedDeviceAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/devices/api/{deviceId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deleted = await dbContext.Devices.FindAsync(deviceId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Delete_NonExistentDevice_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/devices/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ByMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/devices/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> SeedDeviceAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var device = new Device
        {
            Name = "Seeded Device",
            Manufacturer = "Seed Co",
            SerialNumber = "SEED-001",
            PurchaseDate = new DateTime(2025, 12, 1),
            MeasurementType = MeasurementType.ElectricalSignal
        };

        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();
        return device.Id;
    }

    private async Task DeleteDeviceAsync(int deviceId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var device = await dbContext.Devices.FindAsync(deviceId);
        if (device != null)
        {
            dbContext.Devices.Remove(device);
            await dbContext.SaveChangesAsync();
        }
    }
}
