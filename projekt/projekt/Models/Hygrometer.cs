namespace projekt.Models
{
    public class Hygrometer : Device
    {
        public double MinHumidity { get; set; }
        public double MaxHumidity { get; set; }
        public double Accuracy { get; set; }
        public string SensorType { get; set; }

        public Hygrometer()
        {
            MeasurementType = MeasurementType.Humidity;
        }
    }
}
