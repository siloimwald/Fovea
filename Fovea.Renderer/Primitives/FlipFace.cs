using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

public class FlipFace(IPrimitive prim) : IPrimitive
{
    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        if (!prim.Hit(ray, rayInterval, ref hitRecord))
            return false;

        hitRecord.IsFrontFace = !hitRecord.IsFrontFace;

        return true;
    }

    public BoundingBox GetBoundingBox(float t0, float t1)
    {
        return prim.GetBoundingBox(t0, t1);
    }

    public float PdfValue(Vector3 origin, Vector3 direction)
    {
        return prim.PdfValue(origin, direction);
    }

    public Vector3 RandomDirection(Vector3 origin)
    {
        return prim.RandomDirection(origin);
    }
}