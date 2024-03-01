using Fovea.Renderer.Core;

namespace Fovea.Renderer.Sampling;

public readonly struct PrimitivePDF(IPrimitive primitive, Vector3 origin) : IPDF
{
    public float Evaluate(Vector3 direction)
    {
        return primitive.PdfValue(origin, direction);
    }

    public Vector3 Generate()
    {
        return primitive.RandomDirection(origin);
    }
}