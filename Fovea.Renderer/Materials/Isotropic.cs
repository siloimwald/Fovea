using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials;

public class Isotropic(ITexture albedo) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        scatterResult.Attenuation = albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
        // scatterResult.OutgoingRay = new Ray(hitRecord.HitPoint, Sampler.Instance.RandomOnUnitSphere(), rayIn.Time);
        return true;
    }
}