partial class UI : UI<MyViewModel>
{
    BusinessLogic logic;

    public UI() : base()
    {
        logic = new();
    }

    public override void MapPages()
    {
        base.MapPages();

        MapPage("some-page", SomePage);
        MapPage("other-page", OtherPage);
        MapPage("last-page", LastPage);
    }

    void SomePage(Context context)
    {
        context.ViewModel.Count = 999;
    }

    void OtherPage(Context context)
    {
        context.ViewModel.Count = 666;
    }

    void LastPage(Context context)
    {
        context.ViewModel.Count = 333;
    }

    public void StartTimer()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                // using (context.ViewModel.Batch())
                // {
                //     context.ViewModel.Count++;
                //     context.ViewModel.Name = $"Not Rylan Barnes {context.ViewModel.Count}";
                // }
                await Task.Delay(1000);
            }
        });
    }

    public void UpdateTheRecordThings()
    {
        // context.ViewModel.Name = null;
        // context.ViewModel.Count++;

        logic.UpdateTheRecordThings();
    }

    public async Task UpdateTheRecordsAsync()
    {
        Console.WriteLine("Yup, this is executing now...");
        await Task.Delay(3000);
        Console.WriteLine("Yup, done executing.");

        await logic.UpdateTheRecordsAsync(); ;
    }
}