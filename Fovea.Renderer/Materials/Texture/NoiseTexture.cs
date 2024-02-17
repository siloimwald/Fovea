using System;
using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials.Texture;

public class NoiseTexture(float scale) : ITexture
{
    private readonly Perlin _perlin = new();

    public RGBColor Value(float u, float v, Vector3 p)
    {
        var noise = 1 + MathF.Sin(scale * p.Z + _perlin.Turbulence(p) * 10);
        return new RGBColor(1) * (float) (0.5 * noise);
    }
}