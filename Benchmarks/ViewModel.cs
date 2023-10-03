using Xero;

public class ViewModel : IViewModel
{
    public static IViewModel New() => new ViewModel();

    public Action? OnChanged { get; set; }

    public IDisposable Batch()
    {
        throw new NotImplementedException();
    }
}