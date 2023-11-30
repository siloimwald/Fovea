using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core;

public interface IPrimitive
{
    /// <summary>test ray against primitive for intersection</summary>
    /// <param name="ray">ray</param>
    /// <param name="rayInterval">ray interval parameter</param>
    /// <param name="hitRecord">keep track of intersection information</param>
    /// <returns></returns>
    bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord);

    /// <summary>retrieve bounding box for primitive</summary>
    /// <param name="t0">ray time parameter lower bound</param>
    /// <param name="t1">ray time parameter upper bound</param>
    /// <returns></returns>
    BoundingBox GetBoundingBox(float t0, float t1);

    /// <summary>pdf evaluated for the given direction</summary>
    /// <param name="origin">point looking from</param>
    /// <param name="direction">direction towards geometry</param>
    /// <returns></returns>
    float PdfValue(Vector3 origin, Vector3 direction)
    {
        return 0;
    }

    /// <summary>generate a random direction towards the surface of the geometry</summary>
    /// <param name="origin">the point we're looking from</param>
    /// <returns></returns>
    Vector3 RandomDirection(Vector3 origin)
    {
        return Vector3.UnitX;
    }
}