using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public interface IPrimitive
    {
        bool Hit(in Ray ray, float tMin, float tMax, ref HitRecord hitRecord);
        BoundingBox GetBoundingBox();
    }
}