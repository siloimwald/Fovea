using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using static System.Math;

namespace Fovea.Renderer.Materials
{
    public class Dielectric : IMaterial
    {
        private readonly double _ior;

        public Dielectric(double ior)
        {
            _ior = ior;
        }
        
        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            var ratio = hitRecord.IsFrontFace ? (1.0 / _ior) : _ior;
            var unitDirection = Vec3.Normalize(rayIn.Direction);

            var cosTheta = Min(Vec3.Dot(-unitDirection, hitRecord.Normal), 1.0);
            var sinTheta = Sqrt(1.0 - cosTheta * cosTheta);

            var cannotRefract = ratio * sinTheta > 1.0;
            var outDir = cannotRefract || Reflectance(cosTheta, ratio) > Sampler.Instance.Random01()
                            ? Vec3.Reflect(unitDirection, hitRecord.Normal)
                            : Vec3.Refract(unitDirection, hitRecord.Normal, ratio);

            scatterResult.Attenuation = new RGBColor(1.0, 1.0, 1.0);
            scatterResult.OutgoingRay = new Ray(hitRecord.HitPoint, outDir);
            return true;
        }

        // Schlick's approximation
        private double Reflectance(double cosine, double refIdx)
        {
            var r0 = (1.0 - refIdx) / (1.0 + refIdx);
            r0 *= r0;
            return r0 + (1.0 - r0) * Pow(1.0 - cosine, 5.0);
        }
    }
}