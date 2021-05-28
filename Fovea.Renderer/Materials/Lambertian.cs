using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;

namespace Fovea.Renderer.Materials
{
    public class Lambertian : IMaterial
    {
        private readonly ITexture _albedo;

        public Lambertian(ITexture albedo)
        {
            _albedo = albedo;
        }

        public Lambertian(double r, double g, double b):this(new RGBColor(r,g,b))
        {
        }

        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            // book uses "random unit vector", which is a normalized vector from a sample
            // withIN the unit sphere, sampling directly ON the sphere should be the same
            var scatterDirection = hitRecord.Normal + Sampler.Instance.RandomOnUnitSphere();

            if (scatterDirection.IsNearZero())
                scatterDirection = hitRecord.Normal;

            scatterResult.Attenuation = _albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
            scatterResult.OutgoingRay = new Ray(hitRecord.HitPoint, scatterDirection);
            return true;
        }
    }
}