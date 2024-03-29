using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;

namespace Fovea.Renderer.Materials;

public class Isotropic(ITexture albedo) : IMaterial
{
    public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
    {
        scatterResult.Attenuation = albedo.Value(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
        scatterResult.IsSpecular = false;
        scatterResult.Pdf = new SpherePDF();
        return true;
    }

    public float ScatteringPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay) => 1.0f / (4.0f * MathF.PI);
}