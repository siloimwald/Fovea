using Fovea.Renderer.Core;

namespace Fovea.Renderer.Sampling;

public class PrimitivePDF : IPDF
{
    private readonly Vector3 _origin;
    private readonly IPrimitive _primitive;

    public PrimitivePDF(IPrimitive primitive, Vector3 origin)
    {
        _primitive = primitive;
        _origin = origin;
    }

    public float Evaluate(Vector3 direction)
    {
        return _primitive.PdfValue(_origin, direction);
    }

    public Vector3 Generate()
    {
        return _primitive.RandomDirection(_origin);
    }
}