using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;

namespace Sk_WebApi.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    // Enable auto function calling
    private readonly OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    public WeatherForecastController(
        [FromKeyedServices("WeatherAutomationKernel")] Kernel kernel,
        ILogger<WeatherForecastController> logger
        )
    {
        _logger = logger;
        _kernel = kernel;
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    // https://localhost:5173/weatherforecast/average
    [HttpGet("Average")]
    public async Task<string> GetAverageTemp()
    {
        string? input = "Calculate average temperature";

        ChatMessageContent chatResult =
            await _chatCompletionService.GetChatMessageContentAsync(input, openAIPromptExecutionSettings, _kernel);

        return chatResult.ToString();
    }

    // https://localhost:5173/weatherforecast/sum
    [HttpGet("Sum")]
    public async Task<string> GetSumOfTemp()
    {
        string? input = "Calculate sum of temperatures";

        ChatMessageContent chatResult =
            await _chatCompletionService.GetChatMessageContentAsync(input, openAIPromptExecutionSettings, _kernel);

        return chatResult.ToString();
    }























    [HttpGet("All")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}