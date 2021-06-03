using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    /// <summary>
    /// for comparison against our homebrew stuff
    /// </summary>
    public class XZRect : IPrimitive
    {
        private readonly double _x0;
        private readonly double _x1;
        private readonly double _z0;
        private readonly double _z1;
        private readonly double _k;
        private readonly IMaterial _material;

        public XZRect(double x0, double x1, double z0, double z1, double k, IMaterial material)
        {
            _x0 = x0;
            _x1 = x1;
            _z0 = z0;
            _z1 = z1;
            _k = k;
            _material = material;
        }
        
        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var t = (_k - ray.Origin.PY) / ray.Direction.Y;
            if (t < tMin || t > tMax)
                return false;

            var x = ray.Origin.PX + t * ray.Direction.X;
            var z = ray.Origin.PZ + t * ray.Direction.Z;
            if (x < _x0 || x > _x1 || z < _z0 || z > _z1)
                return false;

            hitRecord.TextureU = (x - _x0) / (_x1 - _x0);
            hitRecord.TextureV = (z - _z0) / (_z1 - _z0);
            hitRecord.RayT = t;
            hitRecord.SetFaceNormal(ray, -Vec3.UnitY);
            hitRecord.Material = _material;
            hitRecord.HitPoint = ray.PointsAt(t);
            return true;
        }

        public double PdfValue(Point3 origin, Vec3 direction)
        {
            var hr = new HitRecord();
            if (!Hit(new Ray(origin, direction), 1e-4, double.PositiveInfinity, ref hr))
                return 0;

            var area = (_x1 - _x0) * (_z1 - _z0);
            var distanceSquared = hr.RayT * hr.RayT * direction.LengthSquared();
            var cosine = Math.Abs(Vec3.Dot(direction, hr.Normal) / direction.Length());
            return distanceSquared / (cosine * area);
        }

        public Vec3 RandomDirection(Point3 origin)
        {
            var p = new Point3(Sampler.Instance.Random(_x0, _x1), _k, Sampler.Instance.Random(_z0, _z1));
            return p - origin;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return new(new Point3(_x0, _k - 1e-5, _z0), new Point3(_x1, _k + 1e-5, _z1));
        }
    }
}