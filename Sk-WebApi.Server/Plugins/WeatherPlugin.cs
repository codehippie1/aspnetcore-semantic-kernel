using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Sk_WebApi.Server.Plugins;

/// <summary>
/// Class that represents weather info.
/// </summary>
[Description("Represents weather")]
public class WeatherPlugin()
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [KernelFunction, Description("Returns average temperature")]
    public double Average()
    {
        IEnumerable<WeatherForecast> weatherArray = Enumerable.Range(1, Random.Shared.Next(5, 10)).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        var averageTemp = weatherArray.Select(w => w).Average(x => x.TemperatureC);
        return averageTemp;
    }

    [KernelFunction, Description("Returns sum of temperature")]
    public double Sum()
    {
        IEnumerable<WeatherForecast> weatherArray = Enumerable.Range(1, Random.Shared.Next(5, 10)).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        var sumTemp = weatherArray.Select(w => w).Sum(x => x.TemperatureC);
        return sumTemp;
    }

    [KernelFunction, Description("Returns number of cities we have a temperature measurement")]
    public double Count()
    {
        IEnumerable<WeatherForecast> weatherArray = Enumerable.Range(1, Random.Shared.Next(5, 10)).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        return weatherArray.Count();
    }
}