using Fovea.Renderer.Materials;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Parser.Json;
using SixLabors.ImageSharp.PixelFormats;

namespace Fovea.Renderer.Core;

public struct RGBColor(float r, float g, float b) : ITexture, ITextureGenerator
{
    public float R { get; init; } = r;
    public float G { get; init; } = g;
    public float B { get; init; } = b;

    public RGBColor(float s = 0.0f) : this(s, s, s)
    {
    }

    // addition
    public static RGBColor operator +(RGBColor left, RGBColor right)
    {
        return new(left.R + right.R, left.G + right.G, left.B + right.B);
    }

    // scalar multiplication
    public static RGBColor operator *(RGBColor color, float s)
    {
        return new(color.R * s, color.G * s, color.B * s);
    }

    /// <summary>mix color left and right by component-wise multiplication</summary>
    /// <returns></returns>
    public static RGBColor operator *(RGBColor left, RGBColor right)
    {
        return new(left.R * right.R, left.G * right.G, left.B * right.B);
    }

    public RGBColor Value(float u, float v, Vector3 p)
    {
        return this;
    }

    public static readonly RGBColor White = new(1, 1, 1);
    public static readonly RGBColor Black = new();
    
    public ITexture Generate(ParserContext _) => this;
}

// for the time being, ease the transition, eventually replace RGBColor
public static class RGBColorExtensions 
{
    public static RGBColor FromRgbaVector(this RgbaVector rgbaVector)
    {
        return new RGBColor(rgbaVector.R, rgbaVector.G, rgbaVector.B);
    }
}