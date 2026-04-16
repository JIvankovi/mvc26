using System;

namespace projekt.Models
{
    public class Calibration
    {
        public int Id { get; set; }
        public DateTime CalibrationDateTime { get; set; }
        public Technician Technician { get; set; } = new Technician();
        public string CalibrationStandard { get; set; } = string.Empty;
        public double MeasuredDeviation { get; set; }
        public bool PassedCalibration { get; set; }
        public DateTime NextCalibrationDue { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
