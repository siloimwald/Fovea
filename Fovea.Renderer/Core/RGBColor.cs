using Fovea.Renderer.Materials;
using SixLabors.ImageSharp.PixelFormats;

namespace Fovea.Renderer.Core;

public struct RGBColor : ITexture
{
    public readonly float R;
    public readonly float G;
    public readonly float B;

    public RGBColor(float s = 0.0f) : this(s, s, s)
    {
    }

    public RGBColor(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
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
}

// for the time being, ease the transition, eventually replace RGBColor
public static class RGBColorExtensions 
{
    public static RGBColor FromRgbaVector(this RgbaVector rgbaVector)
    {
        return new RGBColor(rgbaVector.R, rgbaVector.G, rgbaVector.B);
    }
}