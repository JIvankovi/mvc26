using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using projekt.Data;
using projekt.DTOs;
using projekt.Models;
using Xunit;

namespace projekt.Tests;

[Collection("Api integration")]
public class EntityReadDeleteIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private const int MissingId = int.MaxValue;

    private readonly ApiWebApplicationFactory _factory;

    public EntityReadDeleteIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Device_GetById_ReturnsOk()
    {
        var deviceId = await SeedDeviceAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/devices/api/{deviceId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var device = await response.Content.ReadFromJsonAsync<DeviceDto>();
        Assert.NotNull(device);
        Assert.Equal(deviceId, device!.Id);
    }

    [Fact]
    public async Task Device_DeleteById_ReturnsNoContent()
    {
        var deviceId = await SeedDeviceAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/devices/api/{deviceId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Device_GetMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/devices/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Device_DeleteMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/devices/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Laboratory_GetById_ReturnsOk()
    {
        var laboratoryId = await SeedLaboratoryAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/laboratories/api/{laboratoryId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var laboratory = await response.Content.ReadFromJsonAsync<LaboratoryDto>();
        Assert.NotNull(laboratory);
        Assert.Equal(laboratoryId, laboratory!.Id);
    }

    [Fact]
    public async Task Laboratory_DeleteById_ReturnsNoContent()
    {
        var laboratoryId = await SeedLaboratoryAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/laboratories/api/{laboratoryId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Laboratory_GetMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/laboratories/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Laboratory_DeleteMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/laboratories/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Technician_GetById_ReturnsOk()
    {
        var technicianId = await SeedTechnicianAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/technicians/api/{technicianId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var technician = await response.Content.ReadFromJsonAsync<TechnicianDto>();
        Assert.NotNull(technician);
        Assert.Equal(technicianId, technician!.Id);
    }

    [Fact]
    public async Task Technician_DeleteById_ReturnsNoContent()
    {
        var technicianId = await SeedTechnicianAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/technicians/api/{technicianId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Technician_GetMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/technicians/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Technician_DeleteMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/technicians/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Calibration_GetById_ReturnsOk()
    {
        var calibrationId = await SeedCalibrationAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/calibrations/api/{calibrationId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var calibration = await response.Content.ReadFromJsonAsync<CalibrationDto>();
        Assert.NotNull(calibration);
        Assert.Equal(calibrationId, calibration!.Id);
    }

    [Fact]
    public async Task Calibration_DeleteById_ReturnsNoContent()
    {
        var calibrationId = await SeedCalibrationAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/calibrations/api/{calibrationId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Calibration_GetMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/calibrations/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Calibration_DeleteMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/calibrations/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeviceLocation_GetById_ReturnsOk()
    {
        var locationId = await SeedDeviceLocationAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/devicelocations/api/{locationId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var location = await response.Content.ReadFromJsonAsync<DeviceLocationDto>();
        Assert.NotNull(location);
        Assert.Equal(locationId, location!.Id);
    }

    [Fact]
    public async Task DeviceLocation_DeleteById_ReturnsNoContent()
    {
        var locationId = await SeedDeviceLocationAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/devicelocations/api/{locationId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeviceLocation_GetMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/devicelocations/api/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeviceLocation_DeleteMissingId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/devicelocations/api/{MissingId}");

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

    private async Task<int> SeedLaboratoryAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var laboratory = new Laboratory
        {
            Name = "Seeded Laboratory",
            Location = "Building X",
            BuildingCode = "X1",
            RoomNumber = 101,
            ResponsiblePerson = "Lab Owner"
        };

        dbContext.Laboratories.Add(laboratory);
        await dbContext.SaveChangesAsync();
        return laboratory.Id;
    }

    private async Task<int> SeedTechnicianAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var technician = new Technician
        {
            Name = "Seeded Technician",
            Email = "tech@example.com",
            PhoneNumber = "12345",
            Certification = "Certified",
            YearsOfExperience = 5
        };

        dbContext.Technicians.Add(technician);
        await dbContext.SaveChangesAsync();
        return technician.Id;
    }

    private async Task<int> SeedCalibrationAsync()
    {
        var deviceId = await SeedDeviceAsync();
        var technicianId = await SeedTechnicianAsync();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var calibration = new Calibration
        {
            DeviceId = deviceId,
            TechnicianId = technicianId,
            CalibrationDateTime = new DateTime(2026, 6, 11, 10, 0, 0),
            CalibrationStandard = "ISO 17025",
            MeasuredDeviation = 0.01,
            PassedCalibration = true,
            NextCalibrationDue = new DateTime(2027, 6, 11),
            Notes = "Seeded calibration"
        };

        dbContext.Calibrations.Add(calibration);
        await dbContext.SaveChangesAsync();
        return calibration.Id;
    }

    private async Task<int> SeedDeviceLocationAsync()
    {
        var deviceId = await SeedDeviceAsync();
        var laboratoryId = await SeedLaboratoryAsync();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var location = new DeviceLocation
        {
            DeviceId = deviceId,
            LaboratoryId = laboratoryId,
            AssignedDate = new DateTime(2026, 1, 1),
            RemovedDate = null,
            IsCurrentLocation = true,
            AssignmentReason = "Seeded location"
        };

        dbContext.DeviceLocations.Add(location);
        await dbContext.SaveChangesAsync();
        return location.Id;
    }
}