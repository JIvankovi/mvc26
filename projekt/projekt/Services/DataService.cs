using projekt.Models;

namespace projekt.Services
{
    public static class DataService
    {
        public static List<Laboratory> Laboratories { get; set; } = new List<Laboratory>();
        public static List<Device> Devices { get; set; } = new List<Device>();
        public static List<Technician> Technicians { get; set; } = new List<Technician>();
        public static List<Calibration> Calibrations { get; set; } = new List<Calibration>();
        public static List<DeviceLocation> DeviceLocations { get; set; } = new List<DeviceLocation>();
    }
}
