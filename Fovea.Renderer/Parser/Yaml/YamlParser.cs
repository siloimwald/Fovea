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
}