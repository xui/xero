global using Xui.Web.Tags;
global using Xui.Web.ZeroScript;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddXero();

var app = builder.Build();
app.MapUI("/", new UI());

app.Run();