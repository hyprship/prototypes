var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
};

app.MapGet("/weatherforecast", () =>
{
#pragma warning disable SCS0005 // Weak random number generator.
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]))
        .ToArray();
#pragma warning restore SCS0005 // Weak random number generator.
    return forecast;
})
.WithName("GetWeatherForecast");

await app.RunAsync();

#pragma warning disable S3903 // Types should be defined in named namespaces
#pragma warning disable SA1649 // File name should match first type name
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore S3903 // Types should be defined in named namespaces
{
    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);
}