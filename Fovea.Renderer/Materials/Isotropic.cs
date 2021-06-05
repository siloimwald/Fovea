using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials
{
    public class Isotropic : IMaterial
    {
        private readonly ITexture _albedo;

        public Isotropic(ITexture albedo)
        {
            _albedo = albedo;
        }

        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            scatterResult.Attenuation = _albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
            // scatterResult.OutgoingRay = new Ray(hitRecord.HitPoint, Sampler.Instance.RandomOnUnitSphere(), rayIn.Time);
            return true;
        }
    }
}