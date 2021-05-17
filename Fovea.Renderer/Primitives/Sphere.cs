using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Primitives
{
    public class Sphere : IPrimitive
    {
        private readonly Point3 _center;
        private readonly float _radius;

        public Sphere(Point3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        public bool Hit(Ray ray, float tMin, float tMax, HitRecord hitRecord)
        {
            var oc = ray.Origin - _center;
            var a = ray.Direction.LengthSquared();
            var h = Vec3.Dot(oc, ray.Direction); // b=2h
            var c = oc.LengthSquared() - _radius * _radius;
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

            hitRecord.RayT = root;
            hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
            hitRecord.Normal = (hitRecord.HitPoint - _center) * (1.0f / _radius);

            return true;
        }

  
        
    }
}