using System;

namespace projekt.Models
{
    public class Calibration
    {
        public int Id { get; set; }
        public DateTime CalibrationDateTime { get; set; }
        public Technician Technician { get; set; }
        public string CalibrationStandard { get; set; }
        public double MeasuredDeviation { get; set; }
        public bool PassedCalibration { get; set; }
        public DateTime NextCalibrationDue { get; set; }
        public string Notes { get; set; }
    }
}
