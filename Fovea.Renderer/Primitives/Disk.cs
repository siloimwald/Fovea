using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

public class Disk(Vector3 center, Vector3 normal, float radius, IMaterial material)
    : IPrimitive
{
    private readonly Vector3 _normal = Vector3.Normalize(normal);

    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        // intersect with plane disk is in, check radius afterwards
        var denom = Vector3.Dot(_normal, ray.Direction);

        if (Math.Abs(denom) < 1e-6) // parallel 
            return false;

        var tPlane = Vector3.Dot(center - ray.Origin, _normal) / denom;

        if (!rayInterval.Contains(tPlane))
            return false;

        var hp = ray.PointsAt(tPlane);

        // clip against radius
        if ((hp - center).LengthSquared() > radius * radius)
            return false;

        hitRecord.RayT = tPlane;
        hitRecord.Material = material;
        hitRecord.HitPoint = hp;
        hitRecord.SetFaceNormal(ray, _normal);

        return true;
    }

    public BoundingBox GetBoundingBox(float t0, float t1)
    {
        return new(center - new Vector3(radius, radius, radius),
            center + new Vector3(radius, radius, radius));
    }

    public Vector3 RandomDirection(Vector3 origin)
    {
        // this seems to work. x and y seem straightforward, for z no idea why this works :)
        // i would have guessed it should be Length, not Squared, or the distance to the origin or something like that
        var (px, py) = Sampler.Instance.RandomOnUnitDisk();
        px *= radius;
        py *= radius;
        var dir = center - origin;
        var z = dir.LengthSquared();
        return new OrthonormalBasis(dir).Local(px, py, z); 
    }

    public float PdfValue(Vector3 origin, Vector3 direction)
    {
        var hr = new HitRecord();
        if (!Hit(new Ray(origin, direction), Interval.HalfOpenWithOffset(), ref hr))
            return 0;

        var area = radius * radius * Math.PI;
        var distanceSquared = (hr.HitPoint - origin).LengthSquared();
        var cosine = Math.Abs(Vector3.Dot(direction, hr.Normal) / direction.Length());
        var pdfVal = distanceSquared / (cosine * area);
        return (float)pdfVal;
    }
}