partial class ViewModel
{
    public static IViewModel New() => new ViewModel()
    {
        Name = "Rylan Barnes",
        Count = 0,
        Color = "purple",
    };
}