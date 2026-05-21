namespace projekt.ViewModels
{
    public class AutocompleteDropdownViewModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string TextFieldName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int? Value { get; set; }
        public string? SelectedText { get; set; }
        public string Placeholder { get; set; } = "Search...";
        public string AutocompleteUrl { get; set; } = string.Empty;
        public bool Required { get; set; }
    }
}
