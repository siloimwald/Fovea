using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Primitives;

public class MovingSphere(
    Vector3 center0,
    float time0,
    Vector3 center1,
    float time1,
    float radius,
    IMaterial material)
    : IPrimitive
{
    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        var center = CenterAtTime(ray.Time);
        var root = 0.0f;
        if (!Sphere.IntersectSphere(ray, center, radius, rayInterval, ref root))
            return false;

        hitRecord.RayT = root;
        hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
        var outwardNormal = (hitRecord.HitPoint - center) * (1.0f / radius);
        hitRecord.SetFaceNormal(ray, outwardNormal);
        hitRecord.Material = material;

        hitRecord.TextureU = 0.5f + Atan2(outwardNormal.X, outwardNormal.Z) / (2 * PI);
        hitRecord.TextureV = outwardNormal.Y * 0.5f + 0.5f;

        return true;
    }

    public BoundingBox GetBoundingBox(float t0, float t1)
    {
        return BoundingBox.Union(
            Sphere.SphereBox(CenterAtTime(t0), radius),
            Sphere.SphereBox(CenterAtTime(t1), radius));
    }

    private Vector3 CenterAtTime(float time)
    {
        return center0 + (center1 - center0) * ((time - time0) / (time1 - time0));
    }
}