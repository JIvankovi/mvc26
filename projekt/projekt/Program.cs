using projekt.Models;
using projekt.Data;
using Microsoft.EntityFrameworkCore;

namespace projekt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Entity Framework Core with MySQL
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Seed the database with sample data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SeedDatabase(dbContext);
            }

            app.Run();
        }

        private static void SeedDatabase(ApplicationDbContext dbContext)
        {
            // Check if data already exists
            if (dbContext.Technicians.Any())
            {
                Console.WriteLine("Database already seeded with data.");
                return;
            }

            Console.WriteLine("Seeding database with sample data...");

            try
            {
                // Create Technicians
                var technician1 = new Technician
                {
                    Name = "John Smith",
                    Email = "john.smith@lab.com",
                    PhoneNumber = "+1-555-0101",
                    Certification = "ISO 17025 Certified",
                    YearsOfExperience = 8
                };

                var technician2 = new Technician
                {
                    Name = "Maria Garcia",
                    Email = "maria.garcia@lab.com",
                    PhoneNumber = "+1-555-0102",
                    Certification = "NIST Traceable Calibration",
                    YearsOfExperience = 12
                };

                dbContext.Technicians.AddRange(technician1, technician2);
                dbContext.SaveChanges();
                Console.WriteLine("✓ Technicians added");

                // Create Laboratories
                var lab1 = new Laboratory
                {
                    Name = "Environmental Testing Lab",
                    Location = "Building A",
                    BuildingCode = "A",
                    RoomNumber = 101,
                    ResponsiblePerson = "Dr. James Wilson"
                };

                var lab2 = new Laboratory
                {
                    Name = "Electronics & Instrumentation Lab",
                    Location = "Building B",
                    BuildingCode = "B",
                    RoomNumber = 205,
                    ResponsiblePerson = "Prof. Sarah Chen"
                };

                var lab3 = new Laboratory
                {
                    Name = "Metrology Standards Lab",
                    Location = "Building C",
                    BuildingCode = "C",
                    RoomNumber = 310,
                    ResponsiblePerson = "Dr. Michael Brown"
                };

                dbContext.Laboratories.AddRange(lab1, lab2, lab3);
                dbContext.SaveChanges();
                Console.WriteLine("✓ Laboratories added");

                // Create Devices
                var oscilloscope1 = new Oscilloscope
                {
                    Name = "Tektronix MSO64",
                    Manufacturer = "Tektronix",
                    SerialNumber = "OSC-2023-001",
                    PurchaseDate = new DateTime(2023, 3, 15),
                    NumberOfChannels = 4,
                    Bandwidth = 1000.0,
                    SampleRate = 25000.0,
                    DisplayType = "Touchscreen LCD"
                };

                var oscilloscope2 = new Oscilloscope
                {
                    Name = "Keysight DSOX3024T",
                    Manufacturer = "Keysight Technologies",
                    SerialNumber = "OSC-2022-045",
                    PurchaseDate = new DateTime(2022, 11, 20),
                    NumberOfChannels = 4,
                    Bandwidth = 200.0,
                    SampleRate = 5000.0,
                    DisplayType = "Color LCD"
                };

                var barometer1 = new Barometer
                {
                    Name = "Vaisala PTB330",
                    Manufacturer = "Vaisala",
                    SerialNumber = "BAR-2024-012",
                    PurchaseDate = new DateTime(2024, 1, 10),
                    MinPressure = 500.0,
                    MaxPressure = 1100.0,
                    Resolution = 0.01,
                    PressureUnit = "hPa"
                };

                var thermometer1 = new Thermometer
                {
                    Name = "Fluke 1523-P1",
                    Manufacturer = "Fluke Corporation",
                    SerialNumber = "THERM-2023-089",
                    PurchaseDate = new DateTime(2023, 7, 5),
                    MinTemperature = -200.0,
                    MaxTemperature = 300.0,
                    Accuracy = 0.05,
                    Unit = "°C"
                };

                var hygrometer1 = new Hygrometer
                {
                    Name = "Rotronic HygroLog HL-NT",
                    Manufacturer = "Rotronic",
                    SerialNumber = "HYG-2023-034",
                    PurchaseDate = new DateTime(2023, 9, 12),
                    MinHumidity = 0.0,
                    MaxHumidity = 100.0,
                    Accuracy = 0.8,
                    SensorType = "Capacitive"
                };

                var anemometer1 = new Anemometer
                {
                    Name = "Davis Vantage Pro2",
                    Manufacturer = "Davis Instruments",
                    SerialNumber = "ANEM-2024-018",
                    PurchaseDate = new DateTime(2024, 2, 5),
                    MinWindSpeed = 0.0,
                    MaxWindSpeed = 80.0,
                    Accuracy = 0.5,
                    SpeedUnit = "m/s"
                };

                var voltmeter1 = new Voltmeter
                {
                    Name = "Keithley 2110-240",
                    Manufacturer = "Keithley Instruments",
                    SerialNumber = "VOLT-2023-067",
                    PurchaseDate = new DateTime(2023, 5, 18),
                    MinVoltage = 0.0,
                    MaxVoltage = 1000.0,
                    Impedance = 10000000.0,
                    VoltageType = "DC/AC"
                };

                var spectrophotometer1 = new Spectrophotometer
                {
                    Name = "Shimadzu UV-1900",
                    Manufacturer = "Shimadzu",
                    SerialNumber = "SPEC-2022-091",
                    PurchaseDate = new DateTime(2022, 8, 25),
                    MinWavelength = 190.0,
                    MaxWavelength = 1100.0,
                    SpectralBandwidth = 1.0,
                    DetectorType = "Silicon photodiode"
                };

                var voltmeter2 = new Voltmeter
                {
                    Name = "Fluke 87V",
                    Manufacturer = "Fluke Corporation",
                    SerialNumber = "VOLT-2024-003",
                    PurchaseDate = new DateTime(2024, 1, 20),
                    MinVoltage = 0.0,
                    MaxVoltage = 600.0,
                    Impedance = 10000000.0,
                    VoltageType = "True RMS"
                };

                dbContext.Devices.AddRange(oscilloscope1, oscilloscope2, barometer1, thermometer1, 
                                          hygrometer1, anemometer1, voltmeter1, spectrophotometer1, voltmeter2);
                dbContext.SaveChanges();
                Console.WriteLine("✓ Devices added");

                // Create Calibrations
                var calibration1 = new Calibration
                {
                    CalibrationDateTime = new DateTime(2024, 1, 15, 10, 30, 0),
                    Technician = technician1,
                    CalibrationStandard = "ISO/IEC 17025:2017",
                    MeasuredDeviation = 0.02,
                    PassedCalibration = true,
                    NextCalibrationDue = new DateTime(2025, 1, 15),
                    Notes = "All parameters within acceptable range"
                };

                var calibration2 = new Calibration
                {
                    CalibrationDateTime = new DateTime(2024, 2, 20, 14, 0, 0),
                    Technician = technician2,
                    CalibrationStandard = "NIST Handbook 150",
                    MeasuredDeviation = 0.015,
                    PassedCalibration = true,
                    NextCalibrationDue = new DateTime(2025, 2, 20),
                    Notes = "Excellent performance, minimal drift"
                };

                var calibration3 = new Calibration
                {
                    CalibrationDateTime = new DateTime(2024, 3, 10, 9, 15, 0),
                    Technician = technician1,
                    CalibrationStandard = "ISO/IEC 17025:2017",
                    MeasuredDeviation = 0.008,
                    PassedCalibration = true,
                    NextCalibrationDue = new DateTime(2025, 3, 10),
                    Notes = "Calibration successful"
                };

                var calibration4 = new Calibration
                {
                    CalibrationDateTime = new DateTime(2023, 10, 5, 11, 0, 0),
                    Technician = technician2,
                    CalibrationStandard = "ISO/IEC 17025:2017",
                    MeasuredDeviation = 0.012,
                    PassedCalibration = true,
                    NextCalibrationDue = new DateTime(2024, 10, 5),
                    Notes = "Humidity sensor calibrated successfully"
                };

                var calibration5 = new Calibration
                {
                    CalibrationDateTime = new DateTime(2024, 2, 15, 13, 30, 0),
                    Technician = technician1,
                    CalibrationStandard = "NIST Handbook 150",
                    MeasuredDeviation = 0.018,
                    PassedCalibration = true,
                    NextCalibrationDue = new DateTime(2025, 2, 15),
                    Notes = "Wind speed measurements verified"
                };

                oscilloscope1.CalibrationHistory.Add(calibration1);
                barometer1.CalibrationHistory.Add(calibration2);
                thermometer1.CalibrationHistory.Add(calibration3);
                hygrometer1.CalibrationHistory.Add(calibration4);
                anemometer1.CalibrationHistory.Add(calibration5);

                dbContext.Calibrations.AddRange(calibration1, calibration2, calibration3, calibration4, calibration5);
                dbContext.SaveChanges();
                Console.WriteLine("✓ Calibrations added");

                // Create Device Locations
                var locations = new[]
                {
                    new DeviceLocation { Device = oscilloscope1, LaboratoryId = lab2.Id, AssignedDate = new DateTime(2023, 3, 20), IsCurrentLocation = true, AssignmentReason = "Primary oscilloscope for electronics testing" },
                    new DeviceLocation { Device = oscilloscope2, LaboratoryId = lab3.Id, AssignedDate = new DateTime(2022, 11, 25), IsCurrentLocation = true, AssignmentReason = "Standards verification equipment" },
                    new DeviceLocation { Device = barometer1, LaboratoryId = lab1.Id, AssignedDate = new DateTime(2024, 1, 15), IsCurrentLocation = true, AssignmentReason = "Environmental monitoring" },
                    new DeviceLocation { Device = thermometer1, LaboratoryId = lab1.Id, AssignedDate = new DateTime(2023, 7, 10), IsCurrentLocation = true, AssignmentReason = "Temperature calibration reference" },
                    new DeviceLocation { Device = hygrometer1, LaboratoryId = lab1.Id, AssignedDate = new DateTime(2023, 9, 15), IsCurrentLocation = true, AssignmentReason = "Environmental humidity monitoring" },
                    new DeviceLocation { Device = anemometer1, LaboratoryId = lab1.Id, AssignedDate = new DateTime(2024, 2, 10), IsCurrentLocation = true, AssignmentReason = "Wind speed measurements" },
                    new DeviceLocation { Device = voltmeter1, LaboratoryId = lab2.Id, AssignedDate = new DateTime(2023, 5, 25), IsCurrentLocation = true, AssignmentReason = "Precision voltage measurements" },
                    new DeviceLocation { Device = spectrophotometer1, LaboratoryId = lab3.Id, AssignedDate = new DateTime(2022, 9, 1), IsCurrentLocation = true, AssignmentReason = "Optical analysis and spectroscopy" },
                    new DeviceLocation { Device = voltmeter2, LaboratoryId = lab2.Id, AssignedDate = new DateTime(2024, 1, 25), IsCurrentLocation = true, AssignmentReason = "Field measurements and troubleshooting" }
                };

                dbContext.DeviceLocations.AddRange(locations);
                dbContext.SaveChanges();
                Console.WriteLine("✓ Device Locations added");

                Console.WriteLine("\n✓ Database successfully seeded with sample data!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error seeding database: {ex.Message}");
            }
        }

    }
}
