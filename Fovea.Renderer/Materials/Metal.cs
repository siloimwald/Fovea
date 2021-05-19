using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials
{
    public class Metal : IMaterial
    {
        private readonly RGBColor _albedo;
        private readonly float _fuzzy;

        public Metal(RGBColor albedo, float fuzzy = 0.0f)
        {
            _fuzzy = MathF.Min(fuzzy, 1.0f);
            _albedo = albedo;
        }

        public Metal(float r, float g, float b, float fuzzy = 0.0f) : this(new RGBColor(r, g, b), fuzzy)
        {
        }

        public bool Scatter(Ray rayIn, HitRecord hitRecord, ScatterResult scatterResult)
        {
            var reflected = Vec3.Reflect(Vec3.Normalize(rayIn.Direction), hitRecord.Normal);
            scatterResult.Attenuation = _albedo;
            scatterResult.OutgoingRay =
                new Ray(hitRecord.HitPoint, reflected + Sampler.Instance.RandomOnUnitSphere() * _fuzzy);
            return Vec3.Dot(scatterResult.OutgoingRay.Direction, hitRecord.Normal) > 0.0f;
        }
    }
}