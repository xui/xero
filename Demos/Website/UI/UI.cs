using Xero;

partial class UI : UI<ViewModel>
{
    public UI() : base()
    {
    }

    public override void MapPages()
    {
        base.MapPages();

        MapPage("/zero-pages", ZeroPages);
        MapPage("/zero-javascript", ZeroJavaScript);
    }

    [XeroPage("/xero-pages")]
    void ZeroPages(Context context)
    {
        context.ViewModel.Name = "Twas clicked";
        context.ViewModel.ShowAdditional = true;
    }

    [XeroPage("/xero-javascript")]
    void ZeroJavaScript(Context context)
    {
        context.ViewModel.Name = "OK, back to normal: Rylan Barnes";
        context.ViewModel.ShowAdditional = false;
    }

}