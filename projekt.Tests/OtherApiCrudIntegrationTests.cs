using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using projekt.Data;
using projekt.DTOs;
using projekt.Models;
using projekt.ViewModels;
using Xunit;

namespace projekt.Tests;

[Collection("Api integration")]
public class OtherApiCrudIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private const int MissingId = int.MaxValue;
    private const int MissingRelatedId = int.MaxValue - 1;

    private readonly ApiWebApplicationFactory _factory;

    public OtherApiCrudIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Laboratory_GetAll_ReturnsOk()
    {
        var laboratoryId = await SeedLaboratoryAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/laboratories/api");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var laboratories = await response.Content.ReadFromJsonAsync<List<LaboratoryDto>>();
        Assert.NotNull(laboratories);
        Assert.Contains(laboratories!, lab => lab.Id == laboratoryId);

        await DeleteLaboratoryAsync(laboratoryId);
    }

    [Fact]
    public async Task Laboratory_Create_ReturnsCreated()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/laboratories/api", new LaboratoryFormViewModel
        {
            Name = "Integration Lab",
            Location = "Building Z",
            BuildingCode = "Z1",
            RoomNumber = 404,
            ResponsiblePerson = "Dr. Test"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<LaboratoryDto>();
        Assert.NotNull(created);
        Assert.Equal("Integration Lab", created!.Name);

        await DeleteLaboratoryAsync(created.Id);
    }

    [Fact]
    public async Task Laboratory_Create_Invalid_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/laboratories/api", new LaboratoryFormViewModel
        {
            Name = string.Empty,
            Location = "Building Z",
            BuildingCode = "Z1",
            RoomNumber = 404,
            ResponsiblePerson = "Dr. Test"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Laboratory_Update_ReturnsOk()
    {
        var laboratoryId = await SeedLaboratoryAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/laboratories/api/{laboratoryId}", new LaboratoryFormViewModel
        {
            Id = laboratoryId,
            Name = "Updated Lab",
            Location = "Building Y",
            BuildingCode = "Y2",
            RoomNumber = 505,
            ResponsiblePerson = "Updated Person"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await DeleteLaboratoryAsync(laboratoryId);
    }

    [Fact]
    public async Task Laboratory_Update_Invalid_ReturnsBadRequest()
    {
        var laboratoryId = await SeedLaboratoryAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/laboratories/api/{laboratoryId}", new LaboratoryFormViewModel
        {
            Id = laboratoryId,
            Name = string.Empty,
            Location = "Building Y",
            BuildingCode = "Y2",
            RoomNumber = 505,
            ResponsiblePerson = "Updated Person"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await DeleteLaboratoryAsync(laboratoryId);
    }

    [Fact]
    public async Task Technician_GetAll_ReturnsOk()
    {
        var technicianId = await SeedTechnicianAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/technicians/api");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var technicians = await response.Content.ReadFromJsonAsync<List<TechnicianDto>>();
        Assert.NotNull(technicians);
        Assert.Contains(technicians!, tech => tech.Id == technicianId);

        await DeleteTechnicianAsync(technicianId);
    }

    [Fact]
    public async Task Technician_Create_ReturnsCreated()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/technicians/api", new TechnicianFormViewModel
        {
            Name = "Integration Tech",
            Email = "integration.tech@example.com",
            PhoneNumber = "555-1111",
            Certification = "Certified",
            YearsOfExperience = 7
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TechnicianDto>();
        Assert.NotNull(created);
        Assert.Equal("Integration Tech", created!.Name);

        await DeleteTechnicianAsync(created.Id);
    }

    [Fact]
    public async Task Technician_Create_Invalid_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/technicians/api", new TechnicianFormViewModel
        {
            Name = string.Empty,
            Email = "integration.tech@example.com",
            PhoneNumber = "555-1111",
            Certification = "Certified",
            YearsOfExperience = 7
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Technician_Update_ReturnsOk()
    {
        var technicianId = await SeedTechnicianAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/technicians/api/{technicianId}", new TechnicianFormViewModel
        {
            Id = technicianId,
            Name = "Updated Tech",
            Email = "updated.tech@example.com",
            PhoneNumber = "555-2222",
            Certification = "Updated Certified",
            YearsOfExperience = 9
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await DeleteTechnicianAsync(technicianId);
    }

    [Fact]
    public async Task Technician_Update_Invalid_ReturnsBadRequest()
    {
        var technicianId = await SeedTechnicianAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/technicians/api/{technicianId}", new TechnicianFormViewModel
        {
            Id = technicianId,
            Name = string.Empty,
            Email = "updated.tech@example.com",
            PhoneNumber = "555-2222",
            Certification = "Updated Certified",
            YearsOfExperience = 9
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await DeleteTechnicianAsync(technicianId);
    }

    [Fact]
    public async Task Calibration_GetAll_ReturnsOk()
    {
        var calibrationSeed = await SeedCalibrationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/calibrations/api");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var calibrations = await response.Content.ReadFromJsonAsync<List<CalibrationDto>>();
        Assert.NotNull(calibrations);
        Assert.Contains(calibrations!, calibration => calibration.Id == calibrationSeed.CalibrationId);

        await DeleteCalibrationAsync(calibrationSeed.CalibrationId);
        await DeleteDeviceAsync(calibrationSeed.DeviceId);
        await DeleteTechnicianAsync(calibrationSeed.TechnicianId);
    }

    [Fact]
    public async Task Calibration_Create_ReturnsCreated()
    {
        var calibrationSeed = await SeedCalibrationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/calibrations/api", new CalibrationFormViewModel
        {
            DeviceId = calibrationSeed.DeviceId,
            TechnicianId = calibrationSeed.TechnicianId,
            CalibrationDateTime = new DateTime(2026, 6, 11, 10, 30, 0),
            CalibrationStandard = "ISO 17025",
            MeasuredDeviation = 0.05,
            PassedCalibration = true,
            NextCalibrationDue = new DateTime(2027, 6, 11),
            Notes = "Integration calibration"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<CalibrationDto>();
        Assert.NotNull(created);
        Assert.Equal("ISO 17025", created!.CalibrationStandard);

        await DeleteCalibrationAsync(created.Id);
        await DeleteCalibrationAsync(calibrationSeed.CalibrationId);
        await DeleteDeviceAsync(calibrationSeed.DeviceId);
        await DeleteTechnicianAsync(calibrationSeed.TechnicianId);
    }

    [Fact]
    public async Task Calibration_Create_Invalid_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/calibrations/api", new CalibrationFormViewModel
        {
            DeviceId = MissingId,
            TechnicianId = MissingRelatedId,
            CalibrationDateTime = new DateTime(2026, 6, 11, 10, 30, 0),
            CalibrationStandard = "ISO 17025",
            MeasuredDeviation = 0.05,
            PassedCalibration = true,
            NextCalibrationDue = new DateTime(2027, 6, 11),
            Notes = "Invalid references"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Calibration_Update_ReturnsOk()
    {
        var calibrationSeed = await SeedCalibrationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/calibrations/api/{calibrationSeed.CalibrationId}", new CalibrationFormViewModel
        {
            Id = calibrationSeed.CalibrationId,
            DeviceId = calibrationSeed.DeviceId,
            TechnicianId = calibrationSeed.TechnicianId,
            CalibrationDateTime = new DateTime(2026, 6, 12, 12, 0, 0),
            CalibrationStandard = "ISO 17025 Updated",
            MeasuredDeviation = 0.02,
            PassedCalibration = false,
            NextCalibrationDue = new DateTime(2027, 12, 11),
            Notes = "Updated calibration"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await DeleteCalibrationAsync(calibrationSeed.CalibrationId);
        await DeleteDeviceAsync(calibrationSeed.DeviceId);
        await DeleteTechnicianAsync(calibrationSeed.TechnicianId);
    }

    [Fact]
    public async Task Calibration_Update_Invalid_ReturnsBadRequest()
    {
        var calibrationSeed = await SeedCalibrationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/calibrations/api/{calibrationSeed.CalibrationId}", new CalibrationFormViewModel
        {
            Id = calibrationSeed.CalibrationId,
            DeviceId = MissingId,
            TechnicianId = MissingRelatedId,
            CalibrationDateTime = new DateTime(2026, 6, 12, 12, 0, 0),
            CalibrationStandard = "ISO 17025 Updated",
            MeasuredDeviation = 0.02,
            PassedCalibration = false,
            NextCalibrationDue = new DateTime(2027, 12, 11),
            Notes = "Updated calibration"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await DeleteCalibrationAsync(calibrationSeed.CalibrationId);
        await DeleteDeviceAsync(calibrationSeed.DeviceId);
        await DeleteTechnicianAsync(calibrationSeed.TechnicianId);
    }

    [Fact]
    public async Task DeviceLocation_GetAll_ReturnsOk()
    {
        var locationSeed = await SeedDeviceLocationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/devicelocations/api");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var locations = await response.Content.ReadFromJsonAsync<List<DeviceLocationDto>>();
        Assert.NotNull(locations);
        Assert.Contains(locations!, location => location.Id == locationSeed.LocationId);

        await DeleteDeviceLocationAsync(locationSeed.LocationId);
        await DeleteDeviceAsync(locationSeed.DeviceId);
        await DeleteLaboratoryAsync(locationSeed.LaboratoryId!.Value);
    }

    [Fact]
    public async Task DeviceLocation_Create_ReturnsCreated()
    {
        var locationSeed = await SeedDeviceLocationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/devicelocations/api", new DeviceLocationFormViewModel
        {
            DeviceId = locationSeed.DeviceId,
            LaboratoryId = locationSeed.LaboratoryId,
            AssignedDate = new DateTime(2026, 6, 11, 8, 0, 0),
            RemovedDate = null,
            IsCurrentLocation = true,
            AssignmentReason = "Integration assignment"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<DeviceLocationDto>();
        Assert.NotNull(created);
        Assert.Equal(locationSeed.DeviceId, created!.DeviceId);

        await DeleteDeviceLocationAsync(created.Id);
        await DeleteDeviceLocationAsync(locationSeed.LocationId);
        await DeleteDeviceAsync(locationSeed.DeviceId);
        await DeleteLaboratoryAsync(locationSeed.LaboratoryId!.Value);
    }

    [Fact]
    public async Task DeviceLocation_Create_Invalid_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/devicelocations/api", new DeviceLocationFormViewModel
        {
            DeviceId = MissingId,
            LaboratoryId = MissingRelatedId,
            AssignedDate = new DateTime(2026, 6, 11, 8, 0, 0),
            RemovedDate = null,
            IsCurrentLocation = true,
            AssignmentReason = "Invalid assignment"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeviceLocation_Update_ReturnsOk()
    {
        var locationSeed = await SeedDeviceLocationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/devicelocations/api/{locationSeed.LocationId}", new DeviceLocationFormViewModel
        {
            Id = locationSeed.LocationId,
            DeviceId = locationSeed.DeviceId,
            LaboratoryId = locationSeed.LaboratoryId,
            AssignedDate = new DateTime(2026, 6, 12, 9, 0, 0),
            RemovedDate = new DateTime(2026, 6, 13, 9, 0, 0),
            IsCurrentLocation = false,
            AssignmentReason = "Updated assignment"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await DeleteDeviceLocationAsync(locationSeed.LocationId);
        await DeleteDeviceAsync(locationSeed.DeviceId);
        await DeleteLaboratoryAsync(locationSeed.LaboratoryId!.Value);
    }

    [Fact]
    public async Task DeviceLocation_Update_Invalid_ReturnsBadRequest()
    {
        var locationSeed = await SeedDeviceLocationGraphAsync();
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/devicelocations/api/{locationSeed.LocationId}", new DeviceLocationFormViewModel
        {
            Id = locationSeed.LocationId,
            DeviceId = MissingId,
            LaboratoryId = MissingRelatedId,
            AssignedDate = new DateTime(2026, 6, 12, 9, 0, 0),
            RemovedDate = new DateTime(2026, 6, 13, 9, 0, 0),
            IsCurrentLocation = false,
            AssignmentReason = "Updated assignment"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await DeleteDeviceLocationAsync(locationSeed.LocationId);
        await DeleteDeviceAsync(locationSeed.DeviceId);
        await DeleteLaboratoryAsync(locationSeed.LaboratoryId!.Value);
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

    private async Task DeleteLaboratoryAsync(int laboratoryId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var laboratory = await dbContext.Laboratories.FindAsync(laboratoryId);
        if (laboratory != null)
        {
            dbContext.Laboratories.Remove(laboratory);
            await dbContext.SaveChangesAsync();
        }
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

    private async Task DeleteTechnicianAsync(int technicianId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var technician = await dbContext.Technicians.FindAsync(technicianId);
        if (technician != null)
        {
            dbContext.Technicians.Remove(technician);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task<(int CalibrationId, int DeviceId, int TechnicianId)> SeedCalibrationGraphAsync()
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
        return (calibration.Id, deviceId, technicianId);
    }

    private async Task DeleteCalibrationAsync(int calibrationId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var calibration = await dbContext.Calibrations.FindAsync(calibrationId);
        if (calibration != null)
        {
            dbContext.Calibrations.Remove(calibration);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task<(int LocationId, int DeviceId, int? LaboratoryId)> SeedDeviceLocationGraphAsync()
    {
        var deviceId = await SeedDeviceAsync();
        var laboratoryId = await SeedLaboratoryAsync();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var location = new DeviceLocation
        {
            DeviceId = deviceId,
            LaboratoryId = laboratoryId,
            AssignedDate = new DateTime(2026, 6, 11, 8, 0, 0),
            RemovedDate = null,
            IsCurrentLocation = true,
            AssignmentReason = "Seeded assignment"
        };

        dbContext.DeviceLocations.Add(location);
        await dbContext.SaveChangesAsync();
        return (location.Id, deviceId, laboratoryId);
    }

    private async Task DeleteDeviceLocationAsync(int locationId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var location = await dbContext.DeviceLocations.FindAsync(locationId);
        if (location != null)
        {
            dbContext.DeviceLocations.Remove(location);
            await dbContext.SaveChangesAsync();
        }
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