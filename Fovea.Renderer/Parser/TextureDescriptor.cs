using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Materials.Texture;

namespace Fovea.Renderer.Parser;

// a collection of parser entities/descriptors that map to textures/colors
// NOTE: most of these could directly work with the actual classes...

public interface ITextureGenerator
{
    ITexture Generate();
}

public class NoiseTextureDescriptor : ITextureGenerator
{
    public float Scale { get; init; }
    public ITexture Generate() => new NoiseTexture(Scale);
}

public class ColorTextureDescriptor : ITextureGenerator
{
    public RGBColor Color { get; init; }
    public ITexture Generate() => Color;
}

public class ImageTextureDescriptor : ITextureGenerator
{
    public string FileName { get; init; }
    public ITexture Generate() => new ImageTexture(FileName);
}