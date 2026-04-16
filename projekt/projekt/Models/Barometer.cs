namespace projekt.Models
{
    public class Barometer : Device
    {
        public double MinPressure { get; set; }
        public double MaxPressure { get; set; }
        public double Resolution { get; set; }
        public string PressureUnit { get; set; } = string.Empty;

        public Barometer()
        {
            MeasurementType = MeasurementType.Pressure;
        }
    }
}
