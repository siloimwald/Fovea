using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public interface IPrimitive
    {
        bool Hit(Ray ray, float tMin, float tMax, HitRecord hitRecord);
        BoundingBox GetBoundingBox();
    }
}