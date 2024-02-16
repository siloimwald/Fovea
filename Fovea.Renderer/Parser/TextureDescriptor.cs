using Fovea.Renderer.Core;

namespace Fovea.Renderer.Parser;

// a collection of parser entities/descriptors that map to textures/colors

public interface ITextureGenerator
{
    
}

public class NoiseTextureDescriptor : ITextureGenerator
{
    public float Scale { get; init; }
}

public class ColorTextureDescriptor : ITextureGenerator
{
    public RGBColor Color { get; init; }
}

public class ImageTextureDescriptor : ITextureGenerator
{
    public string FileName { get; init; }
}