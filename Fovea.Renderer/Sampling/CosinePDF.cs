using System;

namespace Fovea.Renderer.Sampling;

/// <summary>
/// cosine pdf inlines the ortho-normal base since this turned out to be a huge allocation factor
/// even though it does not affect performance for some reason ;)
/// </summary>
public class CosinePDF : IPDF
{
    private readonly Vector3 _uAxis;
    private readonly Vector3 _vAxis;
    private readonly Vector3 _wAxis;
    
    public CosinePDF(Vector3 normal)
    {
        _wAxis = Vector3.Normalize(normal);
        var a = MathF.Abs(_wAxis.X) > 0.9f ? Vector3.UnitY : Vector3.UnitX;
        _vAxis = Vector3.Normalize(Vector3.Cross(_wAxis, a));
        _uAxis = Vector3.Cross(_wAxis, _vAxis);
    }
    
    public float Evaluate(Vector3 direction)
    {
        var cosine = Vector3.Dot(Vector3.Normalize(direction), _wAxis);
        return cosine < 0 ? 0 : cosine / MathF.PI;
    }

    public Vector3 Generate()
    {
        var randomDirection = Sampler.Instance.RandomCosineDirection();
        return _uAxis * randomDirection.X
               + _vAxis * randomDirection.Y
               + _wAxis * randomDirection.Z;
    }
}