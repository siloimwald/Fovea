using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    // there is some bug somewhere
    public class Cylinder : IPrimitive
    {
        private readonly Point3 _pa;
        private readonly Point3 _pb;
        private readonly double _radius;
        private readonly IMaterial _material;

        public Cylinder(Point3 pa, Point3 pb, double radius, IMaterial material)
        {
            _pa = pa;
            _pb = pb;
            _radius = radius;
            _material = material;
        }
        
        // https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
        // this works, sort of, but cylinder is shifted somehow? leaving it for the time being
        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var ba = _pb - _pa;
            var oc = ray.Origin - _pa;

            var unitDir = Vec3.Normalize(ray.Direction);
            
            var baba = Vec3.Dot(ba, ba);
            var bard = Vec3.Dot(ba, unitDir);
            var baoc = Vec3.Dot(ba, oc);

            var a = baba - bard * bard;
            var b = baba * Vec3.Dot(oc, unitDir) - baoc * bard;
            var c = baba * Vec3.Dot(oc, oc) - baoc * baoc - _radius * _radius * baba;
            var h = b * b - a * c;
            
            if (h < 0)
                return false;

            h = Math.Sqrt(h);
            var t = (-b - h) / a;

            if (t < tMin || tMax < t)
            {
                t = (-b + h) / a;
                if (t < tMin || tMax < t)
                    return false;
            }

            // body hit
            var y = baoc + t * bard;

            if (y > 0 && y < baba)
            {
                hitRecord.RayT = t;
                hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
                var foo =  ba * (y / baba);
                var inner = new Point3(foo.X, foo.Y, foo.Z);
                hitRecord.Normal = Vec3.Normalize((hitRecord.HitPoint - inner) * (1.0 / _radius));
                hitRecord.Material = _material;
                return true;
            }
            
            // caps hit
            t = ((y < 0 ? 0 : baba) - baoc) / bard;
            if (Math.Abs(b + a * t) < h)
            {
                hitRecord.RayT = t;
                hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
                var f = y >= 0 ? 1.0 : -1.0;
                var normal = ba * (f / baba);
                hitRecord.Normal = Vec3.Normalize(normal);
                hitRecord.Material = _material;
                return true;
            }

            return false;
        }

        public BoundingBox GetBoundingBox()
        {
            var min = Point3.Min(_pa, _pb);
            var max = Point3.Max(_pa, _pb);
            var vr = new Vec3(_radius, _radius, _radius);
            return new BoundingBox(min - vr, max + vr);
        }
    }
}