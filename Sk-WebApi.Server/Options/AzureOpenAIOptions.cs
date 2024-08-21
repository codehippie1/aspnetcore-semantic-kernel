using System.ComponentModel.DataAnnotations;

namespace Sk_WebApi.Server.Options;

public sealed class AzureOpenAIOptions
{
    [Required]
    public string ChatDeploymentName { get; set; } = string.Empty;

    [Required]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;
}