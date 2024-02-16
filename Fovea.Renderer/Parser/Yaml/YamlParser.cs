using System.IO;
using System.Xml;
using Fovea.Renderer.Core;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Fovea.Renderer.Parser.Yaml;

public static class YamlParser 
{
    public static IDeserializer Get()
    {
        return new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .WithTagMapping(new TagName("!matte"), typeof(MatteDescriptor))
            .WithTagMapping(new TagName("!metal"), typeof(MetalDescriptor))
            .WithTagMapping(new TagName("!quad"), typeof(QuadDescriptor))
            .WithTagMapping(new TagName("!sphere"), typeof(SphereDescriptor))
            .WithTagMapping(new TagName("!t"), typeof(ImageTextureDescriptor))
            .WithTagMapping(new TagName("!noise"), typeof(NoiseTextureDescriptor))
            .WithTagMapping(new TagName("!ct"), typeof(ColorTextureDescriptor)) // meh...
            .Build();
    }

    public static Scene ParseFile(string fileName)
    {
        using var stream = File.OpenText(fileName);
        return Parse(stream);
    }

    public static Scene ParseYaml(string yaml)
    {
        using var stringReader = new StreamReader(yaml);
        return Parse(stringReader);
    }

    private static Scene Parse(TextReader streamReader)
    {
        var parser = Get();
        var sceneDescriptor = parser.Deserialize<SceneDescriptor>(streamReader);
        return sceneDescriptor.Build();
    }
}