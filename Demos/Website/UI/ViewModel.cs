partial class ViewModel : IViewModel
{
    [Live] string? name;
    [Live] int? count;
    [Live] string? color;
    [Live] bool showAdditional;
    [Live] List<int> colors = new List<int> { 1, 2, 3 };
}