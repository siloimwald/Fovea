using System.IO;
using System.Text.Json;
using Fovea.Renderer.Core;
using Microsoft.Extensions.Logging;

namespace Fovea.Renderer.Parser.Json;

public class JsonParser
{
    private static readonly ILogger<JsonParser> Log = Logging.GetLogger<JsonParser>();

    public static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };


    public static Scene ParseFile(string fileName, OptionsOverride optionOverrides)
    {
        var fullPath = Path.GetFullPath(Path.GetDirectoryName(fileName)!);
        var fileContent = File.ReadAllText(fileName);
        return Parse(fileContent, new ParserContext
        {
            SceneFileLocation = fullPath
        }, optionOverrides);
    }

    private static Scene Parse(string fileContent, ParserContext context, OptionsOverride optionOverrides)
    {
        var sceneDescriptor = JsonSerializer.Deserialize<SceneDescriptor>(fileContent, JsonOptions);
        // try to do some hardening and sanity checking

        if (sceneDescriptor.Materials.Count == 0)
        {
            Log.LogWarning("no materials defined");
        }

        if (sceneDescriptor.Textures.Count == 0)
        {
            Log.LogWarning("no textures defined");
        }
        // need to pass options along since camera construction depends on that
        return sceneDescriptor.Build(context, optionOverrides);
    }

}