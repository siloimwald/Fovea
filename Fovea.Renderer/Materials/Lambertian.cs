using System;
using System.Diagnostics;
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
            var onb = new OrthoNormalBasis(hitRecord.Normal);
            // note: no need to normalize this, book does
            var scatterDirection = onb.Local(Sampler.Instance.RandomCosineDirection());;
            scatterResult.Attenuation = _albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
            scatterResult.OutgoingRay = new Ray(hitRecord.HitPoint, scatterDirection, rayIn.Time);
            scatterResult.PdfValue = Vec3.Dot(onb.WAxis, scatterDirection) / Math.PI;
            return true;
        }

        public double ScatterPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
        {
            // book version used to normalize scatteredRay, but we already do that in Scatter itself. Safe to assume
            // we only ever call this on the same material in succession?
            var cosine = Vec3.Dot(hitRecord.Normal, scatteredRay.Direction);
            return cosine < 0 ? 0 : cosine / Math.PI;
        }
    }
}