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
        private readonly double _fuzzy;

        public Metal(RGBColor albedo, double fuzzy = 0.0)
        {
            _fuzzy = Math.Min(fuzzy, 1.0);
            _albedo = albedo;
        }

        public Metal(double r, double g, double b, double fuzzy = 0.0) : this(new RGBColor(r, g, b), fuzzy)
        {
        }

        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            var reflected = Vec3.Reflect(Vec3.Normalize(rayIn.Direction), hitRecord.Normal);
            scatterResult.Attenuation = _albedo;
            scatterResult.OutgoingRay =
                new Ray(hitRecord.HitPoint, reflected + Sampler.Instance.RandomOnUnitSphere() * _fuzzy);
            return Vec3.Dot(scatterResult.OutgoingRay.Direction, hitRecord.Normal) > 0.0;
        }
    }
}