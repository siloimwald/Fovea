using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Fovea.Renderer.Core;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Fovea.Renderer.Parser.Yaml;

public class YamlParser
{
    private static readonly ILogger<YamlParser> Log = Logging.GetLogger<YamlParser>();
    
    public static IDeserializer GetDeserializer()
    {
        var deserializerBuilder = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties();
 
        return GetMappingList()
            .Aggregate(deserializerBuilder,
                (builder, tag) => builder.WithTagMapping(new TagName(tag.tagName), tag.type))
            .Build();
    }

    public static readonly JsonSerializerOptions JsonOptions =
        new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true 
        };
    
    
    public static ISerializer GetSerializer()
    {
        var serializerBuilder =
            new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .EnsureRoundtrip();

        return GetMappingList()
            .Aggregate(serializerBuilder,
                (builder, tag) => builder.WithTagMapping(new TagName(tag.tagName), tag.type))
            .Build();
    }
    
    public static Scene ParseFile(string fileName)
    {
        var fullPath = Path.GetFullPath(Path.GetDirectoryName(fileName)!);
        var fileContent = File.ReadAllText(fileName);
        return Parse(fileContent, new ParserContext
        {
            SceneFileLocation = fullPath
        });
    }

    private static Scene Parse(string fileContent, ParserContext context)
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

        // if (sceneDescriptor.Lights == null || sceneDescriptor.Lights.Count == 0)
        // {
        //     Log.LogWarning("no light source defined");
        //     throw new NotSupportedException("scene needs at least one light source");
        // }
        
        return sceneDescriptor.Build(context);
    }

    // quick workaround, yaml.net misses a overload for this, or i am not finding it :)
    // otherwise you'd have to respecify tags for both serializer and deserializer
    private static List<(string tagName, Type type)> GetMappingList()
    {
        return new List<(string, Type)>
        {
            ("!matte", typeof(MatteDescriptor)),
            ("!metal", typeof(MetalDescriptor)),
            ("!glass", typeof(DielectricDescriptor)), // not technical glass, but eh
            ("!quad", typeof(QuadDescriptor)),
            ("!ff", typeof(FlipFaceDescriptor)),
            ("!mesh", typeof(MeshFileDescriptor)),
            ("!sphere", typeof(SphereDescriptor)),
            ("!t", typeof(ImageTextureDescriptor)),
            ("!noise", typeof(NoiseTextureDescriptor)),
            ("!diffLight", typeof(DiffuseLightDescriptor))
        };
    }
}