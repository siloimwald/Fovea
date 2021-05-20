using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Materials
{
    public class Dielectric : IMaterial
    {
        private readonly float _ior;

        public Dielectric(float ior)
        {
            _ior = ior;
        }
        
        public bool Scatter(Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            var ratio = hitRecord.IsFrontFace ? (1.0f / _ior) : _ior;
            var unitDirection = Vec3.Normalize(rayIn.Direction);

            var cosTheta = Min(Vec3.Dot(-unitDirection, hitRecord.Normal), 1.0f);
            var sinTheta = Sqrt(1.0f - cosTheta * cosTheta);

            var cannotRefract = ratio * sinTheta > 1.0f;
            var outDir = cannotRefract || Reflectance(cosTheta, ratio) > Sampler.Instance.Random01()
                            ? Vec3.Reflect(unitDirection, hitRecord.Normal)
                            : Vec3.Refract(unitDirection, hitRecord.Normal, ratio);

            scatterResult.Attenuation = new RGBColor(1.0f, 1.0f, 1.0f);
            scatterResult.OutgoingRay = new Ray(hitRecord.HitPoint, outDir);
            return true;
        }

        // Schlick's approximation
        private float Reflectance(float cosine, float refIdx)
        {
            var r0 = (1.0f - refIdx) / (1.0f + refIdx);
            r0 *= r0;
            return r0 + (1.0f - r0) * Pow(1.0f - cosine, 5.0f);
        }
    }
}