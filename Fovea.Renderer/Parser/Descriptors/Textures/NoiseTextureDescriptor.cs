using Fovea.Renderer.Materials;
using Fovea.Renderer.Materials.Texture;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Textures;

public class NoiseTextureDescriptor : ITextureGenerator
{
    public float Scale { get; init; }
    public ITexture Generate(ParserContext context) => new NoiseTexture(Scale);
}