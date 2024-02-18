using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Materials;

public class Dielectric(float ior) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        var ratio = hitRecord.IsFrontFace ? 1.0f / ior : ior;
        var unitDirection = Vector3.Normalize(rayIn.Direction);

        var cosTheta = Min(Vector3.Dot(-unitDirection, hitRecord.Normal), 1.0f);
        var sinTheta = Sqrt(1.0f - cosTheta * cosTheta);

        var cannotRefract = ratio * sinTheta > 1.0f;
        var outDir = cannotRefract || Reflectance(cosTheta, ratio) > Sampler.Instance.Random01()
            ? Vector3.Reflect(unitDirection, hitRecord.Normal)
            : MathUtils.Refract(unitDirection, hitRecord.Normal, ratio);
        
        scatterResult.OutRay = new Ray(hitRecord.HitPoint, outDir);
        scatterResult.IsSpecular = true;
        scatterResult.Pdf = null;
        scatterResult.Attenuation = new RGBColor(1.0f, 1.0f, 1.0f);
        scatterResult.SpecularRay = new Ray(hitRecord.HitPoint, outDir, rayIn.Time);
        return true;
    }

    // Schlick's approximation
    private static float Reflectance(float cosine, float refIdx)
    {
        var r0 = (1.0f - refIdx) / (1.0f + refIdx);
        r0 *= r0;
        return r0 + (1.0f - r0) * Pow(1.0f - cosine, 5.0f);
    }
}