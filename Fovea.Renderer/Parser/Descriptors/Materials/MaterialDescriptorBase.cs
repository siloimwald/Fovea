using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Materials;

namespace Fovea.Renderer.Parser.Descriptors.Materials;

public abstract class MaterialDescriptorBase
{
    /// <summary>
    /// string reference to the texture map within the json file
    /// </summary>
    [JsonPropertyName("texture")]
    public string TextureReference { get; init; } = string.Empty;

    protected ITexture GetTextureOrFail(IDictionary<string, ITexture> textures)
    {
        if (textures.TryGetValue(TextureReference, out var texture))
        {
            return texture;
        }

        throw new SceneReferenceNotFoundException(
            $"{this.GetType().Name} did not find referenced texture {TextureReference}");
    }
}