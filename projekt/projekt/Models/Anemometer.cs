namespace projekt.Models
{
    public class Anemometer : Device
    {
        public double MinWindSpeed { get; set; }
        public double MaxWindSpeed { get; set; }
        public double Accuracy { get; set; }
        public string SpeedUnit { get; set; } = string.Empty;

        public Anemometer()
        {
            MeasurementType = MeasurementType.WindSpeed;
        }
    }
}
