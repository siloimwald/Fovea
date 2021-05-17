namespace Fovea.Renderer.Core
{
    public interface IPrimitive
    {
        bool Hit(Ray ray, float tMin, float tMax, HitRecord hitRecord);
    }
}