using System;
using System.Collections.Generic;

namespace projekt.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public List<Calibration> CalibrationHistory { get; set; }
        public List<DeviceLocation> LocationHistory { get; set; }

        public Device()
        {
            CalibrationHistory = new List<Calibration>();
            LocationHistory = new List<DeviceLocation>();
        }
    }
}
