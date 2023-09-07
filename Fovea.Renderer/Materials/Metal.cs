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
        private readonly float _fuzzy;

        public Metal(ITexture albedo, float fuzzy = 0.0f)
        {
            _fuzzy = MathF.Min(fuzzy, 1.0f);
            _albedo = albedo;
        }

        public Metal(float r, float g, float b, float fuzzy = 0.0f) : this(new RGBColor(r, g, b), fuzzy)
        {
        }

        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            scatterResult.IsSpecular = true;
            scatterResult.Pdf = null;
            scatterResult.Attenuation = _albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);

            var reflected = Vector3.Reflect(Vector3.Normalize(rayIn.Direction), hitRecord.Normal);
            var dir = reflected + Sampler.Instance.RandomOnUnitSphere() * (float)_fuzzy;
            scatterResult.SpecularRay =
                new Ray(hitRecord.HitPoint, dir, rayIn.Time);
            return true;
        }
    }
}