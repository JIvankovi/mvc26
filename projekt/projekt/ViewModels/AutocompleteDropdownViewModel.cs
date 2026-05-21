namespace projekt.ViewModels
{
    public class AutocompleteDropdownViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string HiddenFieldName { get; set; } = string.Empty;
        public int? HiddenValue { get; set; }
        public string DisplayValue { get; set; } = string.Empty;
        public string SearchUrl { get; set; } = string.Empty;
        public string Placeholder { get; set; } = "Start typing...";
        public bool Required { get; set; }
    }
}
