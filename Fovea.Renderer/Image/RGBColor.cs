using Fovea.Renderer.Materials;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Image;

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

    /// <summary>clamps color components to [0..1], scales by 255 and packs components as an byte array with form [r,g,b]</summary>
    /// <returns></returns>
    public byte[] ToByteArray()
    {
        return new[]
        {
            (byte) (MathUtils.ClampF(R, 0.0f, 1.0f) * 255.999f),
            (byte) (MathUtils.ClampF(G, 0.0f, 1.0f) * 255.999f),
            (byte) (MathUtils.ClampF(B, 0.0f, 1.0f) * 255.999f)
        };
    }

    public RGBColor Value(float u, float v, Vector3 p)
    {
        return this;
    }
}