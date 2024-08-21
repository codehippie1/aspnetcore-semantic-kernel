using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Sk_WebApi.Server.Plugins;

/// <summary>
/// Simple plugin that just returns the time.
/// </summary>
public class MyTimePlugin
{
    [KernelFunction, Description("Get the current time")]
    public DateTimeOffset Time() => DateTimeOffset.Now;
}