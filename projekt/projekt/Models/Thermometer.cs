namespace projekt.Models
{
    public class Thermometer : Device
    {
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double Accuracy { get; set; }
        public string Unit { get; set; } = string.Empty;

        public Thermometer()
        {
            MeasurementType = MeasurementType.Temperature;
        }
    }
}
