using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using static System.Math;

namespace Fovea.Renderer.Primitives
{
    public class Sphere : IPrimitive
    {
        private readonly Point3 _center;
        private readonly double _radius;
        private readonly IMaterial _material;

        public Sphere(Point3 center, double radius, IMaterial material)
        {
            _center = center;
            _radius = radius;
            _material = material;
        }

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var root = 0.0;
            if (!IntersectSphere(ray, _center, _radius, tMin, tMax, ref root))
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

        public static bool IntersectSphere(
            in Ray ray, in Point3 center, double radius,
            double tMin, double tMax, ref double tRay)
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
            if (root < tMin || tMax < root) // check the other one if this is outside our range
            {
                root = (-h + discSqrt) / a;
                if (root < tMin || tMax < root)
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
        
        public BoundingBox GetBoundingBox(double t0, double t1) => SphereBox(_center, _radius);
    }
}