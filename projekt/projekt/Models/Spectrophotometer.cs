namespace projekt.Models
{
    public class Spectrophotometer : Device
    {
        public double MinWavelength { get; set; }
        public double MaxWavelength { get; set; }
        public double SpectralBandwidth { get; set; }
        public string DetectorType { get; set; } = string.Empty;

        public Spectrophotometer()
        {
            MeasurementType = MeasurementType.LightSpectrum;
        }
    }
}
