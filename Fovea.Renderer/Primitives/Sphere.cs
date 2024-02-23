using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Primitives;

public class Sphere(Vector3 center, float radius, IMaterial material) : IPrimitive
{
    private readonly Vector3 _centerVec; // vector from center0 to center1 for moving sphere
    private readonly bool _isMoving = false;
    
    /// <summary>
    /// moving sphere constructor
    /// </summary>
    /// <param name="center">center for time 0</param>
    /// <param name="center1">center for time 1</param>
    /// <param name="radius">radius</param>
    /// <param name="material">sphere material</param>
    public Sphere(Vector3 center, Vector3 center1, float radius, IMaterial material)
        :this(center, radius, material)
    {
        _centerVec = center1 - center;
        _isMoving = true;
    }
    
    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        var root = 0.0f;

        var sphereCenter = _isMoving ? CenterAtTime(ray.Time) : center;
        
        if (!IntersectSphere(ray, sphereCenter, radius, rayInterval, ref root))
            return false;

        hitRecord.RayT = root;
        hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
        var outwardNormal = (hitRecord.HitPoint - sphereCenter) * (1.0f / radius);
        hitRecord.SetFaceNormal(ray, outwardNormal);
        hitRecord.Material = material;

        hitRecord.TextureU = 0.5f + Atan2(outwardNormal.X, outwardNormal.Z) / (2 * PI);
        hitRecord.TextureV = outwardNormal.Y * 0.5f + 0.5f;

        return true;
    }

    public BoundingBox GetBoundingBox()
    {
        var rVec = new Vector3(radius, radius, radius);
        if (_isMoving)
        {
            return BoundingBox.Union(
                new BoundingBox(center - rVec, center + rVec),
                new BoundingBox(_centerVec + center - rVec, _centerVec + center + rVec));
        }

        return new BoundingBox(center - rVec, center + rVec);

    }

    public float PdfValue(Vector3 origin, Vector3 direction)
    {
        var hr = new HitRecord();
        if (!Hit(new Ray(origin, direction), Interval.HalfOpenWithOffset(), ref hr))
            return 0;
     
        var cosTheta = Sqrt(1.0f - radius * radius / (center - origin).LengthSquared());
        var solidAngle = 2.0f * PI * (1.0f - cosTheta);
        return (1.0f / solidAngle);
    }

    public Vector3 RandomDirection(Vector3 origin)
    {
        var dir = center - origin;
        var distanceSquared = dir.LengthSquared();
        var r1 = Sampler.Instance.Random01();
        var r2 = Sampler.Instance.Random01();
        var z = 1.0f + r2 * (Sqrt(1.0f - radius * radius / distanceSquared) - 1.0f);
        var phi = 2.0f * PI * r1;
        var x = Cos(phi) * Sqrt(1.0f - z * z);
        var y = Sin(phi) * Sqrt(1.0f - z * z);
        return new OrthonormalBasis(dir).Local(x, y, z);
    }

    public static bool IntersectSphere(
        in Ray ray, in Vector3 center, float radius,
        in Interval rayInterval, ref float tRay)
    {
        var oc = ray.Origin - center;
        var a = ray.Direction.LengthSquared();
        var h = Vector3.Dot(oc, ray.Direction); // b=2h
        var c = oc.LengthSquared() - radius * radius;
        var disc = h * h - a * c;

        if (disc < 0)
            return false;

        var discSqrt = Sqrt(disc);

        var root = (-h - discSqrt) / a;
        if (!rayInterval.Contains(root)) // check the other one if this is outside our range
        {
            root = (-h + discSqrt) / a;
            if (!rayInterval.Contains(root))
                return false;
        }

        tRay = root;
        return true;
    }


    /// <summary>
    /// linearly interpolate sphere center depending on time
    /// </summary>
    /// <param name="t">ray time</param>
    /// <returns>sphere center time t</returns>
    private Vector3 CenterAtTime(float t)
    {
        return center + _centerVec * t;
    }
}