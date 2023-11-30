using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials;

public class DiffuseLight : IMaterial
{
    private readonly ITexture _color;

    public DiffuseLight(ITexture color)
    {
        _color = color;
    }

    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        return false;
    }

    public RGBColor Emitted(in Ray ray, in HitRecord hitRecord)
    {
        return hitRecord.IsFrontFace
            ? _color.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint)
            : new RGBColor();
    }
}