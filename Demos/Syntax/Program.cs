using Xero;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddXero();

var app = builder.Build();
app.MapUI("/", new UI());

app.Run();