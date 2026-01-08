var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

// I already Click yes to install certificates for http requests.
// https://www.youtube.com/watch?v=2c99pIfZq1s