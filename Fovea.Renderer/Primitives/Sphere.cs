﻿using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using static System.Math;

namespace Fovea.Renderer.Primitives
{
    public class Sphere : IPrimitive
    {
        private readonly Point3 _center;
        private readonly IMaterial _material;
        private readonly double _radius;

        public Sphere(Point3 center, double radius, IMaterial material)
        {
            _center = center;
            _radius = radius;
            _material = material;
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            var root = 0.0;
            if (!IntersectSphere(ray, _center, _radius, rayInterval, ref root))
                return false;

            hitRecord.RayT = root;
            hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
            var outwardNormal = (hitRecord.HitPoint - _center) * (1.0 / _radius);
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = _material;

            hitRecord.TextureU = 0.5 + Atan2(outwardNormal.X, outwardNormal.Z) / (2 * PI);
            hitRecord.TextureV = outwardNormal.Y * 0.5 + 0.5;

            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return SphereBox(_center, _radius);
        }

        public float PdfValue(Vector3 origin, Vector3 direction)
        {
            var hr = new HitRecord();
            if (!Hit(new Ray(origin, direction), Interval.HalfOpenWithOffset(), ref hr))
                return 0;

            var cosTheta = Sqrt(1.0 - _radius * _radius / (_center - origin.AsPoint3()).LengthSquared());
            var solidAngle = 2.0 * PI * (1.0 - cosTheta);
            return (float)(1.0 / solidAngle);
        }

        public Vector3 RandomDirection(Vector3 origin)
        {
            var dir = _center.AsVector3() - origin;
            var distanceSquared = dir.LengthSquared();
            var r1 = Sampler.Instance.Random01();
            var r2 = Sampler.Instance.Random01();
            var z = 1.0 + r2 * (Sqrt(1.0 - _radius * _radius / distanceSquared) - 1.0);
            var phi = 2.0 * PI * r1;
            var x = Cos(phi) * Sqrt(1.0 - z * z);
            var y = Sin(phi) * Sqrt(1.0 - z * z);
            return new OrthonormalBasis(dir).Local(x, y, z);
        }

        public static bool IntersectSphere(
            in Ray ray, in Point3 center, double radius,
            in Interval rayInterval, ref double tRay)
        {
            var oc = ray.Origin - center;
            var a = ray.Direction.LengthSquared();
            var h = Vec3.Dot(oc, ray.Direction); // b=2h
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

        public static BoundingBox SphereBox(Point3 center, double radius)
        {
            return new(center - new Vec3(radius, radius, radius),
                center + new Vec3(radius, radius, radius));
        }
    }
}