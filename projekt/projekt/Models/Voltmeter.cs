namespace projekt.Models
{
    public class Voltmeter : Device
    {
        public double MinVoltage { get; set; }
        public double MaxVoltage { get; set; }
        public double Impedance { get; set; }
        public string VoltageType { get; set; }

        public Voltmeter()
        {
            MeasurementType = MeasurementType.Voltage;
        }
    }
}
