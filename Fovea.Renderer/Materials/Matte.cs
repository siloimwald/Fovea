using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials;

public class Matte(ITexture albedo) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        
        scatterResult.IsSpecular = false;
        scatterResult.Attenuation = albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);

        var onb = new OrthonormalBasis(hitRecord.Normal);
        var scatterDirection = onb.Local(Sampler.Instance.RandomCosineDirection());
        scatterResult.OutRay = 
            new Ray(hitRecord.HitPoint, scatterDirection, rayIn.Time);
        scatterResult.Pdf = Vector3.Dot(onb.WAxis, scatterDirection) / MathF.PI;
        
        return true;
    }

    public float ScatteringPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
    {
        var cosine = Vector3.Dot(hitRecord.Normal, Vector3.Normalize(scatteredRay.Direction));
        return cosine < 0 ? 0 : cosine / MathF.PI;
    }
}