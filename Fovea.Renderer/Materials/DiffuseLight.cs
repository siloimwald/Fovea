using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials;

public class DiffuseLight(ITexture color, bool flipNormal = false) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        return false;
    }

    public RGBColor Emitted(in Ray ray, in HitRecord hitRecord)
    {
        var notFrontFace = !hitRecord.IsFrontFace;
        if (flipNormal)
        {
            notFrontFace = !notFrontFace;
        }
        return notFrontFace ? RGBColor.Black : color.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
    }
}