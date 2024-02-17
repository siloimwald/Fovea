using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fovea.Renderer.Core;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Fovea.Renderer.Parser.Yaml;

public static class YamlParser 
{
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
        using var stream = File.OpenText(fileName);
        return Parse(stream);
    }

    public static Scene ParseYaml(string yaml)
    {
        using var stringReader = new StringReader(yaml);
        return Parse(stringReader);
    }

    private static Scene Parse(TextReader streamReader)
    {
        var parser = GetDeserializer();
        var sceneDescriptor = parser.Deserialize<SceneDescriptor>(streamReader);
        return sceneDescriptor.Build();
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
            ("!diffLight", typeof(DiffuseLightDescriptor)),
            ("!ct", typeof(ColorTextureDescriptor)), // meh, directly to RGB would be neat
        };
    }
}