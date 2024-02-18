using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;

namespace Fovea.Renderer.Materials;

public class Matte(ITexture albedo) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        
        scatterResult.IsSpecular = false;
        scatterResult.Attenuation = albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
        scatterResult.Pdf = new CosinePDF(hitRecord.Normal);

        // book 1 drop in
        scatterResult.OutRay = 
            new Ray(hitRecord.HitPoint,
                hitRecord.Normal + Sampler.Instance.RandomCosineDirection(),
                rayIn.Time);
        
        return true;
    }

    public float ScatteringPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
    {
        var cosine = Vector3.Dot(hitRecord.Normal, Vector3.Normalize(scatteredRay.Direction));
        return cosine < 0 ? 0 : cosine / MathF.PI;
    }
}