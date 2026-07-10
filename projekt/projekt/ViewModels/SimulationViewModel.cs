using System.Collections.Generic;

namespace projekt.ViewModels
{
    public class SimulationLaboratoryViewModel
    {
        public int LaboratoryId { get; set; }
        public string LaboratoryName { get; set; } = string.Empty;
        public List<SimulationDeviceReadingViewModel> Devices { get; set; } = new();
    }

    public class SimulationDeviceReadingViewModel
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public double ReadingValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string MeasurementType { get; set; } = string.Empty;
    }
}
