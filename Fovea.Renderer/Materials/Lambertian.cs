using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials
{
    public class Lambertian : IMaterial
    {
        private readonly ITexture _albedo;

        public Lambertian(ITexture albedo)
        {
            _albedo = albedo;
        }

        public Lambertian(double r, double g, double b) : this(new RGBColor(r, g, b))
        {
        }

        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            scatterResult.IsSpecular = false;
            scatterResult.Attenuation = _albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
            scatterResult.Pdf = new CosinePDF(new OrthonormalBasis(hitRecord.Normal.AsVector3()));
            return true;
        }

        public double ScatteringPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
        {
            var cosine = Vec3.Dot(hitRecord.Normal, Vec3.Normalize(scatteredRay.Direction));
            return cosine < 0 ? 0 : cosine / Math.PI;
        }
    }
}