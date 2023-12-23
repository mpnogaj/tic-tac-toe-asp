var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/ping", () => "Hello World!");

app.Run();