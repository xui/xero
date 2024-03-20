global using Xui.Web.Html;
global using Xui.Web.ZeroScript;
global using Xui.Web.HttpX;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddXui();
builder.Services.AddLogging(c => c.ClearProviders()); // TODO: Remove

var app = builder.Build();
app.MapUI("/", new UI());

app.Run();