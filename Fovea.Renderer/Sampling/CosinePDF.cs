using System;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Sampling;

public class CosinePDF : IPDF
{
    private readonly OrthonormalBasis _onb;
    
    public CosinePDF(Vector3 normal)
    {
        _onb = new OrthonormalBasis(normal);
    }
    
    public float Evaluate(Vector3 direction)
    {
        var cosine = Vector3.Dot(Vector3.Normalize(direction), _onb.WAxis);
        return cosine < 0 ? 0 : cosine / MathF.PI;
    }

    public Vector3 Generate() => _onb.Local(Sampler.Instance.RandomCosineDirection());
}