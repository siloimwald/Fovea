using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials
{
    public class Metal : IMaterial
    {
        private readonly ITexture _albedo;
        private readonly double _fuzzy;

        public Metal(ITexture albedo, double fuzzy = 0.0)
        {
            _fuzzy = Math.Min(fuzzy, 1.0);
            _albedo = albedo;
        }

        public Metal(double r, double g, double b, double fuzzy = 0.0) : this(new RGBColor(r, g, b), fuzzy)
        {
        }

        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            scatterResult.IsSpecular = true;
            scatterResult.Pdf = null;
            scatterResult.Attenuation = _albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
            var reflected = Vec3.Reflect(Vec3.Normalize(rayIn.Direction), hitRecord.Normal);
            scatterResult.SpecularRay =
                new Ray(hitRecord.HitPoint, reflected + Sampler.Instance.RandomOnUnitSphere() * _fuzzy, rayIn.Time);
            return true;
        }
    }
}