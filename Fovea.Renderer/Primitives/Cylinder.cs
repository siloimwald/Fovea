using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

/// <summary>
/// cylinder along the z axis
/// </summary>
/// <param name="zMin">z min coordinate</param>
/// <param name="zMax">z max coordinate</param>
/// <param name="radius">radius of caps in z plane</param>
/// <param name="material">material</param>
public class Cylinder(float zMin, float zMax, float radius, IMaterial material)
    : IPrimitive
{
    private readonly float _zMax = Math.Max(zMin, zMax);
    private readonly float _zMin = Math.Min(zMin, zMax);

    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        // get better cap hit, if any
        var tCap = 0.0f;
        var hitCap = TestCapHit(ray, rayInterval, ref tCap);
        var tBody = 0.0f;
        var hitBody = TestBodyHit(ray, rayInterval, ref tBody);

        if (!hitBody && !hitCap)
            return false;

        if (hitBody && !hitCap || hitBody && tBody < tCap)
        {
            var hitPoint = ray.PointsAt(tBody);
            var n = (hitPoint - new Vector3(0, 0, hitPoint.Z)) * (1.0f / radius);
            hitRecord.HitPoint = hitPoint;
            hitRecord.Material = material;
            hitRecord.RayT = tBody;

            var theta = MathF.Atan2(n.X, n.Y);
            hitRecord.TextureV = 0.5f + theta / (2 * MathF.PI);
            hitRecord.TextureU = (hitPoint.Z - _zMin) / (_zMax - _zMin);

            hitRecord.SetFaceNormal(ray, n);
            return true;
        }


        // cap hit
        hitRecord.HitPoint = ray.PointsAt(tCap);
        hitRecord.RayT = tCap;
        hitRecord.Material = material;

        hitRecord.TextureU = 0.5f + hitRecord.HitPoint.X / radius * 0.5f;
        hitRecord.TextureV = 0.5f + hitRecord.HitPoint.Y / radius * 0.5f;

        // flip the normal accordingly
        var s = hitRecord.HitPoint.Z < 0 ? -1 : 1;
        hitRecord.SetFaceNormal(ray, new Vector3(0, 0, s));
        return true;
    }

    public BoundingBox GetBoundingBox()
    {
        var min = new Vector3(-radius, -radius, _zMin);
        var max = new Vector3(radius, radius, _zMax);
        return new BoundingBox(min, max);
    }

    /// <summary>returns true if we hit either of the caps</summary>
    /// <param name="ray">ray</param>
    /// <param name="rayInterval">ray interval</param>
    /// <param name="tCap">reference to better cap hit</param>
    /// <returns></returns>
    private bool TestCapHit(in Ray ray, in Interval rayInterval, ref float tCap)
    {
        // parallel to plane
        if (MathF.Abs(ray.Direction.Z) < 1e-6f)
            return false;

        var t0 = (_zMin - ray.Origin.Z) / ray.Direction.Z;
        var t1 = (_zMax - ray.Origin.Z) / ray.Direction.Z;

        if (t0 > t1) MathUtils.Swap(ref t0, ref t1);

        tCap = t0;
        var hp = ray.PointsAt(tCap);
        var r2 = radius * radius;
        if (rayInterval.Contains(tCap) && !(hp.X * hp.X + hp.Y * hp.Y > r2)) return true;
        tCap = t1;
        hp = ray.PointsAt(tCap);

        return rayInterval.Contains(tCap) && !(hp.X * hp.X + hp.Y * hp.Y > r2);
    }

    /// <summary>test ray against cylinder body</summary>
    /// <param name="ray">incoming ray</param>
    /// <param name="rayInterval">ray interval</param>
    /// <param name="tBody">potential closest body hit</param>
    /// <returns></returns>
    private bool TestBodyHit(in Ray ray, in Interval rayInterval, ref float tBody)
    {
        var a = ray.Direction.X * ray.Direction.X + ray.Direction.Y * ray.Direction.Y;
        var b = 2.0 * (ray.Direction.X * ray.Origin.X + ray.Direction.Y * ray.Origin.Y);
        var c = ray.Origin.X * ray.Origin.X + ray.Origin.Y * ray.Origin.Y - radius * radius;

        float t0 = 0.0f, t1 = 0.0f;
        if (!MathUtils.SolveQuadratic(a, (float)b,c, ref t0, ref t1))
            return false;

        tBody = t0;
        // test against ray interval and clip
        var hpz = ray.Origin.Z + tBody * ray.Direction.Z;
        if (!rayInterval.Contains(tBody) || hpz < _zMin || hpz > _zMax)
        {
            tBody = t1;
            hpz = ray.Origin.Z + tBody * ray.Direction.Z;
            if (!rayInterval.Contains(tBody) || hpz < _zMin || hpz > _zMax)
                return false;
        }

        return true;
    }
}