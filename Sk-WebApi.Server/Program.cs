// https://devblogs.microsoft.com/semantic-kernel/how-to-get-started-using-semantic-kernel-net/
// https://github.com/arashjalalat/semantic-kernel-dotnet-minimal-api
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Sk_WebApi.Server;
using Sk_WebApi.Server.Options;
using Sk_WebApi.Server.Plugins;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//// Actual code to execute is found in Worker class
//builder.Services.AddHostedService<Worker>();

// Get configuration
builder.Services.AddOptions<AzureOpenAIOptions>()
                .Bind(builder.Configuration.GetSection(nameof(AzureOpenAIOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

// Chat completion service that kernels will use
builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    AzureOpenAIOptions options = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

    // A custom HttpClient can be provided to this constructor
    return new AzureOpenAIChatCompletionService(options.ChatDeploymentName, options.Endpoint, options.ApiKey);

    /* Alternatively, you can use plain, non-Azure OpenAI after loading OpenAIOptions instead
       of AzureOpenAI options with builder.Services.AddOptions:
    OpenAI options = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;

    return new OpenAIChatCompletionService(options.ChatModelId, options.ApiKey);*/
});

// Add plugins that can be used by kernels
// The plugins are added as singletons so that they can be used by multiple kernels
builder.Services.AddSingleton<MyTimePlugin>();
builder.Services.AddSingleton<MyAlarmPlugin>();
builder.Services.AddKeyedSingleton<MyLightPlugin>("OfficeLight");
builder.Services.AddKeyedSingleton<MyLightPlugin>("PorchLight", (sp, key) =>
{
    return new MyLightPlugin(turnedOn: true);
});
// +++++++++++++++++++++++
builder.Services.AddSingleton<WeatherPlugin>();

/* To add an OpenAI or OpenAPI plugin, you need to be using Microsoft.SemanticKernel.Plugins.OpenApi.
   Then create a temporary kernel, use it to load the plugin and add it as keyed singleton.
Kernel kernel = new();
KernelPlugin openAIPlugin = await kernel.ImportPluginFromOpenAIAsync("<plugin name>", new Uri("<OpenAI-plugin>"));
builder.Services.AddKeyedSingleton<KernelPlugin>("MyImportedOpenAIPlugin", openAIPlugin);

KernelPlugin openApiPlugin = await kernel.ImportPluginFromOpenApiAsync("<plugin name>", new Uri("<OpenAPI-plugin>"));
builder.Services.AddKeyedSingleton<KernelPlugin>("MyImportedOpenApiPlugin", openApiPlugin);*/

// Add a home automation kernel to the dependency injection container
builder.Services.AddKeyedTransient<Kernel>("HomeAutomationKernel", (sp, key) =>
{
    // Create a collection of plugins that the kernel will use
    KernelPluginCollection pluginCollection = [];
    pluginCollection.AddFromObject(sp.GetRequiredService<MyTimePlugin>());
    pluginCollection.AddFromObject(sp.GetRequiredService<MyAlarmPlugin>());
    pluginCollection.AddFromObject(sp.GetRequiredKeyedService<MyLightPlugin>("OfficeLight"), "OfficeLight");
    pluginCollection.AddFromObject(sp.GetRequiredKeyedService<MyLightPlugin>("PorchLight"), "PorchLight");
    // +++++++++++++++++++++++
    pluginCollection.AddFromObject(sp.GetRequiredService<WeatherPlugin>());

    // When created by the dependency injection container, Semantic Kernel logging is included by default
    return new Kernel(sp, pluginCollection);
});

// Add a weather automation kernel to the dependency injection container
builder.Services.AddKeyedTransient<Kernel>("WeatherAutomationKernel", (sp, key) =>
{
    // Create a collection of plugins that the kernel will use
    KernelPluginCollection pluginCollection = [];
    pluginCollection.AddFromObject(sp.GetRequiredService<WeatherPlugin>());
    // When created by the dependency injection container, Semantic Kernel logging is included by default
    return new Kernel(sp, pluginCollection);
});


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
