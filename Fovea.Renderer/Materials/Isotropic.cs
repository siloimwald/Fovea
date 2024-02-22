using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;

namespace Fovea.Renderer.Materials;

public class Isotropic(ITexture albedo) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        scatterResult.Attenuation = albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
        scatterResult.OutRay = new Ray(hitRecord.HitPoint, Sampler.Instance.RandomOnUnitSphere(), rayIn.Time);
        return true;
    }
}