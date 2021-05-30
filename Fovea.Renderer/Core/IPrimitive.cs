using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public interface IPrimitive
    {
        bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord);
        BoundingBox GetBoundingBox(double t0, double t1);
    }
}