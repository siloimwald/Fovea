using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

/// <summary>
/// quadrilateral from book 2, Section 6 
/// </summary>
public class Quad : IPrimitive
{
    private readonly float _distance;
    private readonly Vector3 _normal;
    private readonly Vector3 _point;
    private readonly Vector3 _uDirection;
    private readonly Vector3 _vDirection;
    private readonly Vector3 _w;
    private readonly IMaterial _material;
    private readonly float _area;
    
    /// <summary>
    /// quadrilateral from book 2, Section 6 
    /// </summary>
    public Quad(Vector3 point, Vector3 uDirection, Vector3 vDirection, IMaterial material)
    {
        _point = point;
        _uDirection = uDirection;
        _vDirection = vDirection;
        _material = material;
        var uCrossV = Vector3.Cross(uDirection, vDirection);
        _normal = Vector3.Normalize(uCrossV);
        _area = uCrossV.Length();
        _w = uCrossV / uCrossV.LengthSquared();
        _distance = Vector3.Dot(_normal, _point);
    }

    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        var denominator = Vector3.Dot(_normal, ray.Direction);

        // parallel to plane
        if (MathF.Abs(denominator) < 1e-6)
        {
            return false;
        }

        var t = (_distance - Vector3.Dot(_normal, ray.Origin)) / denominator;
        // outside ray interval
        if (!rayInterval.Contains(t))
            return false;

        // parameterization of point within plane
        var intersection = ray.PointsAt(t);
        var planarHit = intersection - _point;
        var alpha = Vector3.Dot(_w, Vector3.Cross(planarHit, _vDirection));
        var beta = Vector3.Dot(_w, Vector3.Cross(_uDirection, planarHit));

        // not within quad
        if (alpha < 0 || alpha > 1 || beta < 0 || beta > 1)
            return false;

        hitRecord.TextureU = alpha;
        hitRecord.TextureV = beta;
        hitRecord.RayT = t;
        hitRecord.HitPoint = intersection;
        hitRecord.Material = _material;
        hitRecord.SetFaceNormal(ray, _normal);
        
        return true;
    }

    public BoundingBox GetBoundingBox()
    {
        // the point opposite to point
        var q2 = _point + _uDirection + _vDirection;
        var min = Vector3.Min(_point, q2);
        var max = Vector3.Max(_point, q2);
        return new BoundingBox(min, max).Padded();
    }

    public float PdfValue(Vector3 origin, Vector3 direction)
    {
        var hr = new HitRecord();
        if (!Hit(new Ray(origin, direction), Interval.HalfOpenWithOffset(), ref hr))
        {
            return 0.0f;
        }

        var distanceSquared = hr.RayT * hr.RayT * direction.LengthSquared();
        var cosine = MathF.Abs(Vector3.Dot(direction, hr.Normal) / direction.Length());

        return distanceSquared / (cosine * _area);
    }

    public Vector3 RandomDirection(Vector3 origin)
    {
        var p = _point +
                (Sampler.Instance.Random01() * _uDirection)
                + (Sampler.Instance.Random01() * _vDirection);
        return p - origin;
    }
}