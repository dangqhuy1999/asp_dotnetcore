var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!"); // Visible middleware component

//basic routing 
// the middleware component is going to use it to handle all requests
// http requests body is empty in this case, use for PUT/POST with body content
app.Run(async (HttpContext context) =>
{
    // context.Request.Method
    await context.Response.WriteAsync($"The method is: {context.Request.Method}\r\n");
    await context.Response.WriteAsync($"The URL is: {context.Request.Path}\r\n");

    await context.Response.WriteAsync($"\r\nHeader: \r\n");

    foreach (var key in context.Request.Headers.Keys)
    {
        await context.Response.WriteAsync($"{key} : {context.Request.Headers[key]}\r\n");
    }

    // in browser, you can see the result
    // with http://localhost:5145/ or http://localhost:5145/test and so on.
    // although there is no visible middleware component.
});

app.Run();

// I already Click yes to install certificates for http requests.
// https://www.youtube.com/watch?v=2c99pIfZq1s