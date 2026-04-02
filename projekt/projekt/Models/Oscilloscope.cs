namespace projekt.Models
{
    public class Oscilloscope : Device
    {
        private enum Resolution
        {
            EightBit,
            TenBit,
            TwelveBit,
            FourteenBit,
            SixteenBit
        }

        public int NumberOfChannels { get; set; }
        public double Bandwidth { get; set; }
        public double SampleRate { get; set; }
        public string DisplayType { get; set; }
        private Resolution DeviceResolution { get; set; }

        public Oscilloscope()
        {
            MeasurementType = MeasurementType.ElectricalSignal;
        }
    }
}
