using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class Disk : IPrimitive
    {
        private readonly Point3 _center;
        private readonly IMaterial _material;
        private readonly Vec3 _normal;
        private readonly double _radius;

        public Disk(Point3 center, Vec3 normal, double radius, IMaterial material)
        {
            _center = center;
            _normal = Vec3.Normalize(normal);
            _radius = radius;
            _material = material;
        }

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            // intersect with plane disk is in, check radius afterwards
            var denom = Vec3.Dot(_normal, ray.Direction);

            if (Math.Abs(denom) < 1e-6) // parallel 
                return false;

            var tPlane = Vec3.Dot(_center - ray.Origin, _normal) / denom;

            if (tPlane < tMin || tMax < tPlane)
                return false;

            var hp = ray.PointsAt(tPlane);

            // clip against radius
            if ((hp - _center).LengthSquared() > _radius * _radius)
                return false;

            hitRecord.RayT = tPlane;
            hitRecord.Material = _material;
            hitRecord.HitPoint = hp;
            hitRecord.SetFaceNormal(ray, _normal);

            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return new(_center - new Vec3(_radius, _radius, _radius),
                _center + new Vec3(_radius, _radius, _radius));
        }

        public Vec3 RandomDirection(Point3 origin)
        {
            // this seems to work. x and y seem straightforward, for z no idea why this works :)
            // i would have guessed it should be Length, not Squared, or the distance to the origin or something like that
            var (px, py) = Sampler.Instance.RandomOnUnitDisk();
            px *= _radius;
            py *= _radius;
            var dir = _center - origin;
            var z = dir.LengthSquared();
            return new OrthoNormalBasis(dir).Local(px, py, z);
        }

        public double PdfValue(Point3 origin, Vec3 direction)
        {
            var hr = new HitRecord();
            if (!Hit(new Ray(origin, direction), 1e-4, double.PositiveInfinity, ref hr))
                return 0;

            var area = _radius * _radius * Math.PI;
            var distanceSquared = (hr.HitPoint - origin).LengthSquared();
            var cosine = Math.Abs(Vec3.Dot(direction, hr.Normal) / direction.Length());
            var pdfVal = distanceSquared / (cosine * area);
            return pdfVal;
        }
    }
}