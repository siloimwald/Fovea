using System.IO;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Materials.Texture;
using Fovea.Renderer.Parser.Yaml;

namespace Fovea.Renderer.Parser;

// a collection of parser entities/descriptors that map to textures/colors
// NOTE: most of these could directly work with the actual classes...

public interface ITextureGenerator
{
    ITexture Generate(ParserContext context);
}

public class NoiseTextureDescriptor : ITextureGenerator
{
    public float Scale { get; init; }
    public ITexture Generate(ParserContext context) => new NoiseTexture(Scale);
}

public class ColorTextureDescriptor : ITextureGenerator
{
    public RGBColor Color { get; init; }
    public ITexture Generate(ParserContext context) => Color;
}

public class ImageTextureDescriptor : ITextureGenerator
{
    public string FileName { get; init; }

    public ITexture Generate(ParserContext context) => new ImageTexture(Path.Combine(context.SceneFileLocation, FileName));
}