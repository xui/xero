using Xero;

partial class MyViewModel
{
    public static IViewModel New() => new MyViewModel()
    {
        Name = "Rylan Barnes",
        Count = 0,
        Color = "purple",
        Sub = new()
        {
            First = "Rylan",
            Last = "Barnes",
            Email = "rylan@barn.es",
            Password = "1234",
            Quantity = 4,
        }
    };
}