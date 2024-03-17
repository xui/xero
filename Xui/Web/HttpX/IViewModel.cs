namespace Xui.Web.HttpX;

public interface IViewModel
{
    static abstract IViewModel New();
    Action? OnChanged { get; set; }
    IDisposable Batch();
}