using Microsoft.Extensions.Logging;

namespace Fovea.Renderer.Core;

public static class Logging
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory
            .Create(builder => 
                builder.AddSimpleConsole(cfg =>
                {
                    cfg.IncludeScopes = true;
                    cfg.SingleLine = true;
                    cfg.TimestampFormat = "HH:mm:ss ";
                }));
    
    public static ILogger<T> GetLogger<T>() => LoggerFactory.CreateLogger<T>();
}