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
        // scatterResult.Pdf = new CosinePDF(hitRecord.Normal);

        var outDirection = hitRecord.Normal + Sampler.Instance.RandomOnUnitSphere();

        if (MathF.Abs(outDirection.X) < 1e-6f && MathF.Abs(outDirection.Y) < 1e-6f && MathF.Abs(outDirection.Z) < 1e-6f)
            outDirection = hitRecord.Normal;
        
        // book 1 drop in
        scatterResult.OutRay = 
            new Ray(hitRecord.HitPoint, outDirection, rayIn.Time);
        
        return true;
    }

    public float ScatteringPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
    {
        var cosine = Vector3.Dot(hitRecord.Normal, Vector3.Normalize(scatteredRay.Direction));
        return cosine < 0 ? 0 : cosine / MathF.PI;
    }
}