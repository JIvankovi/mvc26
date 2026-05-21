using System;

namespace projekt.ViewModels
{
    public class DateTimeControlViewModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public DateTime? Value { get; set; }
        public bool Required { get; set; }
    }
}
