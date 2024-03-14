global using Xui.Web.Tags;
global using Xui.Web.ZeroScript;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddXero();
builder.Services.AddLogging(c => c.ClearProviders()); // TODO: Remove

var app = builder.Build();
app.MapUI("/", new UI());

app.Run();