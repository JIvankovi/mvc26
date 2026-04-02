using projekt.Models;
using projekt.Services;

namespace projekt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Initialize sample data
            InitializeSampleData();

            app.Run();
        }

        private static void InitializeSampleData()
        {
            // Create Technicians
            var technician1 = new Technician
            {
                Id = 1,
                Name = "John Smith",
                Email = "john.smith@lab.com",
                PhoneNumber = "+1-555-0101",
                Certification = "ISO 17025 Certified",
                YearsOfExperience = 8
            };

            var technician2 = new Technician
            {
                Id = 2,
                Name = "Maria Garcia",
                Email = "maria.garcia@lab.com",
                PhoneNumber = "+1-555-0102",
                Certification = "NIST Traceable Calibration",
                YearsOfExperience = 12
            };

            // Create Laboratories
            var lab1 = new Laboratory
            {
                Id = 1,
                Name = "Environmental Testing Lab",
                Location = "Building A",
                BuildingCode = "A",
                RoomNumber = 101,
                ResponsiblePerson = "Dr. James Wilson"
            };

            var lab2 = new Laboratory
            {
                Id = 2,
                Name = "Electronics & Instrumentation Lab",
                Location = "Building B",
                BuildingCode = "B",
                RoomNumber = 205,
                ResponsiblePerson = "Prof. Sarah Chen"
            };

            var lab3 = new Laboratory
            {
                Id = 3,
                Name = "Metrology Standards Lab",
                Location = "Building C",
                BuildingCode = "C",
                RoomNumber = 310,
                ResponsiblePerson = "Dr. Michael Brown"
            };

            // Create Devices
            var oscilloscope1 = new Oscilloscope
            {
                Id = 1,
                Name = "Tektronix MSO64",
                Manufacturer = "Tektronix",
                SerialNumber = "OSC-2023-001",
                PurchaseDate = new DateTime(2023, 3, 15),
                NumberOfChannels = 4,
                Bandwidth = 1000.0, // MHz
                SampleRate = 25000.0, // MS/s
                DisplayType = "Touchscreen LCD"
            };

            var oscilloscope2 = new Oscilloscope
            {
                Id = 2,
                Name = "Keysight DSOX3024T",
                Manufacturer = "Keysight Technologies",
                SerialNumber = "OSC-2022-045",
                PurchaseDate = new DateTime(2022, 11, 20),
                NumberOfChannels = 4,
                Bandwidth = 200.0, // MHz
                SampleRate = 5000.0, // MS/s
                DisplayType = "Color LCD"
            };

            var barometer1 = new Barometer
            {
                Id = 3,
                Name = "Vaisala PTB330",
                Manufacturer = "Vaisala",
                SerialNumber = "BAR-2024-012",
                PurchaseDate = new DateTime(2024, 1, 10),
                MinPressure = 500.0, // hPa
                MaxPressure = 1100.0, // hPa
                Resolution = 0.01,
                PressureUnit = "hPa"
            };

            var thermometer1 = new Thermometer
            {
                Id = 4,
                Name = "Fluke 1523-P1",
                Manufacturer = "Fluke Corporation",
                SerialNumber = "THERM-2023-089",
                PurchaseDate = new DateTime(2023, 7, 5),
                MinTemperature = -200.0, // °C
                MaxTemperature = 300.0, // °C
                Accuracy = 0.05,
                Unit = "°C"
            };

            var hygrometer1 = new Hygrometer
            {
                Id = 5,
                Name = "Rotronic HygroLog HL-NT",
                Manufacturer = "Rotronic",
                SerialNumber = "HYG-2023-034",
                PurchaseDate = new DateTime(2023, 9, 12),
                MinHumidity = 0.0, // %RH
                MaxHumidity = 100.0, // %RH
                Accuracy = 0.8,
                SensorType = "Capacitive"
            };

            var anemometer1 = new Anemometer
            {
                Id = 6,
                Name = "Davis Vantage Pro2",
                Manufacturer = "Davis Instruments",
                SerialNumber = "ANEM-2024-018",
                PurchaseDate = new DateTime(2024, 2, 5),
                MinWindSpeed = 0.0, // m/s
                MaxWindSpeed = 80.0, // m/s
                Accuracy = 0.5,
                SpeedUnit = "m/s"
            };

            var voltmeter1 = new Voltmeter
            {
                Id = 7,
                Name = "Keithley 2110-240",
                Manufacturer = "Keithley Instruments",
                SerialNumber = "VOLT-2023-067",
                PurchaseDate = new DateTime(2023, 5, 18),
                MinVoltage = 0.0, // V
                MaxVoltage = 1000.0, // V
                Impedance = 10000000.0, // 10 MΩ
                VoltageType = "DC/AC"
            };

            var spectrophotometer1 = new Spectrophotometer
            {
                Id = 8,
                Name = "Shimadzu UV-1900",
                Manufacturer = "Shimadzu",
                SerialNumber = "SPEC-2022-091",
                PurchaseDate = new DateTime(2022, 8, 25),
                MinWavelength = 190.0, // nm
                MaxWavelength = 1100.0, // nm
                SpectralBandwidth = 1.0,
                DetectorType = "Silicon photodiode"
            };

            var voltmeter2 = new Voltmeter
            {
                Id = 9,
                Name = "Fluke 87V",
                Manufacturer = "Fluke Corporation",
                SerialNumber = "VOLT-2024-003",
                PurchaseDate = new DateTime(2024, 1, 20),
                MinVoltage = 0.0, // V
                MaxVoltage = 600.0, // V
                Impedance = 10000000.0, // 10 MΩ
                VoltageType = "True RMS"
            };

            // Create Calibrations
            var calibration1 = new Calibration
            {
                Id = 1,
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
                Id = 2,
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
                Id = 3,
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
                Id = 4,
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
                Id = 5,
                CalibrationDateTime = new DateTime(2024, 2, 15, 13, 30, 0),
                Technician = technician1,
                CalibrationStandard = "NIST Handbook 150",
                MeasuredDeviation = 0.018,
                PassedCalibration = true,
                NextCalibrationDue = new DateTime(2025, 2, 15),
                Notes = "Wind speed measurements verified"
            };

            // Add calibrations to devices
            oscilloscope1.CalibrationHistory.Add(calibration1);
            barometer1.CalibrationHistory.Add(calibration2);
            thermometer1.CalibrationHistory.Add(calibration3);
            hygrometer1.CalibrationHistory.Add(calibration4);
            anemometer1.CalibrationHistory.Add(calibration5);

            // Create Device Locations (N-N relationship)
            var deviceLocation1 = new DeviceLocation
            {
                Id = 1,
                DeviceId = oscilloscope1.Id,
                Device = oscilloscope1,
                LaboratoryId = lab2.Id,
                Laboratory = lab2,
                AssignedDate = new DateTime(2023, 3, 20),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Primary oscilloscope for electronics testing"
            };

            var deviceLocation2 = new DeviceLocation
            {
                Id = 2,
                DeviceId = oscilloscope2.Id,
                Device = oscilloscope2,
                LaboratoryId = lab3.Id,
                Laboratory = lab3,
                AssignedDate = new DateTime(2022, 11, 25),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Standards verification equipment"
            };

            var deviceLocation3 = new DeviceLocation
            {
                Id = 3,
                DeviceId = barometer1.Id,
                Device = barometer1,
                LaboratoryId = lab1.Id,
                Laboratory = lab1,
                AssignedDate = new DateTime(2024, 1, 15),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Environmental monitoring"
            };

            var deviceLocation4 = new DeviceLocation
            {
                Id = 4,
                DeviceId = thermometer1.Id,
                Device = thermometer1,
                LaboratoryId = lab1.Id,
                Laboratory = lab1,
                AssignedDate = new DateTime(2023, 7, 10),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Temperature calibration reference"
            };

            // Historical location - thermometer was previously in lab3
            var deviceLocation5 = new DeviceLocation
            {
                Id = 5,
                DeviceId = thermometer1.Id,
                Device = thermometer1,
                LaboratoryId = lab3.Id,
                Laboratory = lab3,
                AssignedDate = new DateTime(2023, 7, 10),
                RemovedDate = new DateTime(2023, 12, 15),
                IsCurrentLocation = false,
                AssignmentReason = "Temporary assignment for cross-calibration"
            };

            var deviceLocation6 = new DeviceLocation
            {
                Id = 6,
                DeviceId = hygrometer1.Id,
                Device = hygrometer1,
                LaboratoryId = lab1.Id,
                Laboratory = lab1,
                AssignedDate = new DateTime(2023, 9, 15),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Environmental humidity monitoring"
            };

            var deviceLocation7 = new DeviceLocation
            {
                Id = 7,
                DeviceId = anemometer1.Id,
                Device = anemometer1,
                LaboratoryId = lab1.Id,
                Laboratory = lab1,
                AssignedDate = new DateTime(2024, 2, 10),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Wind speed measurements for environmental studies"
            };

            var deviceLocation8 = new DeviceLocation
            {
                Id = 8,
                DeviceId = voltmeter1.Id,
                Device = voltmeter1,
                LaboratoryId = lab2.Id,
                Laboratory = lab2,
                AssignedDate = new DateTime(2023, 5, 25),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Precision voltage measurements"
            };

            var deviceLocation9 = new DeviceLocation
            {
                Id = 9,
                DeviceId = spectrophotometer1.Id,
                Device = spectrophotometer1,
                LaboratoryId = lab3.Id,
                Laboratory = lab3,
                AssignedDate = new DateTime(2022, 9, 1),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Optical analysis and spectroscopy"
            };

            var deviceLocation10 = new DeviceLocation
            {
                Id = 10,
                DeviceId = voltmeter2.Id,
                Device = voltmeter2,
                LaboratoryId = lab2.Id,
                Laboratory = lab2,
                AssignedDate = new DateTime(2024, 1, 25),
                RemovedDate = null,
                IsCurrentLocation = true,
                AssignmentReason = "Field measurements and troubleshooting"
            };

            // Add locations to devices and laboratories
            oscilloscope1.LocationHistory.Add(deviceLocation1);
            oscilloscope2.LocationHistory.Add(deviceLocation2);
            barometer1.LocationHistory.Add(deviceLocation3);
            thermometer1.LocationHistory.Add(deviceLocation4);
            thermometer1.LocationHistory.Add(deviceLocation5);
            hygrometer1.LocationHistory.Add(deviceLocation6);
            anemometer1.LocationHistory.Add(deviceLocation7);
            voltmeter1.LocationHistory.Add(deviceLocation8);
            spectrophotometer1.LocationHistory.Add(deviceLocation9);
            voltmeter2.LocationHistory.Add(deviceLocation10);

            lab1.DeviceLocations.Add(deviceLocation3);
            lab1.DeviceLocations.Add(deviceLocation4);
            lab1.DeviceLocations.Add(deviceLocation6);
            lab1.DeviceLocations.Add(deviceLocation7);
            lab2.DeviceLocations.Add(deviceLocation1);
            lab2.DeviceLocations.Add(deviceLocation8);
            lab2.DeviceLocations.Add(deviceLocation10);
            lab3.DeviceLocations.Add(deviceLocation2);
            lab3.DeviceLocations.Add(deviceLocation5);
            lab3.DeviceLocations.Add(deviceLocation9);

            // Store data in DataService for access throughout the application
            DataService.Laboratories.AddRange(new[] { lab1, lab2, lab3 });
            DataService.Devices.AddRange(new Device[] { oscilloscope1, oscilloscope2, barometer1, thermometer1, 
                                                 hygrometer1, anemometer1, voltmeter1, spectrophotometer1, voltmeter2 });
            DataService.Technicians.AddRange(new[] { technician1, technician2 });
            DataService.Calibrations.AddRange(new[] { calibration1, calibration2, calibration3, calibration4, calibration5 });
            DataService.DeviceLocations.AddRange(new[] { deviceLocation1, deviceLocation2, deviceLocation3, deviceLocation4, 
                                                         deviceLocation5, deviceLocation6, deviceLocation7, deviceLocation8, 
                                                         deviceLocation9, deviceLocation10 });

            // ===========================================
            // LINQ QUERY DEMONSTRATIONS
            // ===========================================
            Console.WriteLine("\n========================================");
            Console.WriteLine("=== LINQ QUERY DEMONSTRATIONS ===");
            Console.WriteLine("========================================\n");

            // LINQ Query 1: Find all devices in Building C
            Console.WriteLine("1. Devices in Building C:");
            var devicesInBuildingC = DataService.Laboratories
                .Where(lab => lab.BuildingCode == "C")
                .SelectMany(lab => lab.DeviceLocations)
                .Where(dl => dl.IsCurrentLocation)
                .Select(dl => dl.Device)
                .ToList();
            
            foreach (var device in devicesInBuildingC)
            {
                Console.WriteLine($"   - {device.Name} ({device.GetType().Name})");
            }

            // LINQ Query 2: Oscilloscopes with 10-bit resolution in Building C (hypothetical)
            // Note: Since we don't set DeviceResolution (it's private), we'll demonstrate the query structure
            Console.WriteLine("\n2. Oscilloscopes in Building C:");
            var oscilloscopesInBuildingC = DataService.Laboratories
                .Where(lab => lab.BuildingCode == "C")
                .SelectMany(lab => lab.DeviceLocations)
                .Where(dl => dl.IsCurrentLocation && dl.Device is Oscilloscope)
                .Select(dl => dl.Device as Oscilloscope)
                .ToList();
            
            foreach (var osc in oscilloscopesInBuildingC)
            {
                Console.WriteLine($"   - {osc.Name}: {osc.NumberOfChannels} channels, {osc.Bandwidth} MHz bandwidth");
            }

            // LINQ Query 3: Group devices by manufacturer and count
            Console.WriteLine("\n3. Devices grouped by Manufacturer:");
            var devicesByManufacturer = DataService.Devices
                .GroupBy(d => d.Manufacturer)
                .Select(g => new { Manufacturer = g.Key, Count = g.Count(), Devices = g.ToList() })
                .OrderByDescending(x => x.Count)
                .ToList();
            
            foreach (var group in devicesByManufacturer)
            {
                Console.WriteLine($"   - {group.Manufacturer}: {group.Count} device(s)");
                foreach (var device in group.Devices)
                {
                    Console.WriteLine($"     • {device.Name}");
                }
            }

            Console.WriteLine("\n========================================\n");

            // Display summary
            Console.WriteLine("=== Sample Data Initialized ===");
            Console.WriteLine($"\nLaboratories created: {3}");
            Console.WriteLine($"- {lab1.Name} ({lab1.Location}) - {lab1.DeviceLocations.Count(dl => dl.IsCurrentLocation)} current devices");
            Console.WriteLine($"- {lab2.Name} ({lab2.Location}) - {lab2.DeviceLocations.Count(dl => dl.IsCurrentLocation)} current devices");
            Console.WriteLine($"- {lab3.Name} ({lab3.Location}) - {lab3.DeviceLocations.Count(dl => dl.IsCurrentLocation)} current devices");

            Console.WriteLine($"\nDevices created: {9}");
            Console.WriteLine($"- {oscilloscope1.Name} (ID: {oscilloscope1.Id}) - in {lab2.Name}");
            Console.WriteLine($"- {oscilloscope2.Name} (ID: {oscilloscope2.Id}) - in {lab3.Name}");
            Console.WriteLine($"- {barometer1.Name} (ID: {barometer1.Id}) - in {lab1.Name}");
            Console.WriteLine($"- {thermometer1.Name} (ID: {thermometer1.Id}) - in {lab1.Name}");
            Console.WriteLine($"- {hygrometer1.Name} (ID: {hygrometer1.Id}) - in {lab1.Name}");
            Console.WriteLine($"- {anemometer1.Name} (ID: {anemometer1.Id}) - in {lab1.Name}");
            Console.WriteLine($"- {voltmeter1.Name} (ID: {voltmeter1.Id}) - in {lab2.Name}");
            Console.WriteLine($"- {spectrophotometer1.Name} (ID: {spectrophotometer1.Id}) - in {lab3.Name}");
            Console.WriteLine($"- {voltmeter2.Name} (ID: {voltmeter2.Id}) - in {lab2.Name}");

            Console.WriteLine($"\nTechnicians created: {2}");
            Console.WriteLine($"- {technician1.Name} ({technician1.Certification})");
            Console.WriteLine($"- {technician2.Name} ({technician2.Certification})");

            Console.WriteLine($"\nCalibrations performed: {5}");
            Console.WriteLine($"\nDevice locations assigned: {10}");
            Console.WriteLine("\n=================================\n");
        }
    }
}
