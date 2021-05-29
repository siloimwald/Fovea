using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class Disk : IPrimitive
    {
        private readonly Point3 _center;
        private readonly Vec3 _normal;
        private readonly double _radius;
        private readonly IMaterial _material;

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
            if ((hp - _center).LengthSquared() > _radius*_radius)
                return false;

            hitRecord.RayT = tPlane;
            hitRecord.Material = _material;
            hitRecord.HitPoint = hp;
            hitRecord.SetFaceNormal(ray, _normal);

            return true;
        }

        public BoundingBox GetBoundingBox()
        {
            // this is very conservative...
            return new(_center - new Vec3(_radius, _radius, _radius),
                _center + new Vec3(_radius, _radius, _radius));
        }
    }
}