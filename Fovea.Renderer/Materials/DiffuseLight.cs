using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials;

public class DiffuseLight(ITexture color) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        return false;
    }

    public RGBColor Emitted(in Ray ray, in HitRecord hitRecord)
    {
        return !hitRecord.IsFrontFace ? RGBColor.Black : color.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
    }
}