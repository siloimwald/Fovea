using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;
using YamlDotNet.Serialization;

namespace Fovea.Renderer.Parser;

// a collection of parser entities/descriptors that map to materials

public interface IMaterialGenerator
{
    IMaterial Generate(IDictionary<string, ITexture> textures);
}

public abstract class MaterialDescriptorBase
{
    /// <summary>
    /// string reference to the texture map within the yaml file
    /// </summary>
    [YamlMember(Alias = "texture")]
    public string TextureReference { get; init; } = string.Empty;

    protected ITexture GetTextureOrFail(IDictionary<string, ITexture> textures)
    {
        if (textures.TryGetValue(TextureReference, out var texture))
        {
            return texture;
        }

        throw new SceneReferenceNotFoundException(
            $"{this.GetType().Name} did not find referenced material {TextureReference}");
    }
}

public class MatteDescriptor : MaterialDescriptorBase, IMaterialGenerator
{
    public IMaterial Generate(IDictionary<string, ITexture> textures)
        => new Matte(GetTextureOrFail(textures));
}

public class DielectricDescriptor : IMaterialGenerator
{
    [YamlMember(Alias = "ior")]
    public float IOR { get; init; }

    public IMaterial Generate(IDictionary<string, ITexture> textures) => new Dielectric(IOR);
}

public class MetalDescriptor : MaterialDescriptorBase, IMaterialGenerator
{
    public float Fuzzy { get; init; }

    public IMaterial Generate(IDictionary<string, ITexture> textures)
        => new Metal(GetTextureOrFail(textures), Fuzzy);
}