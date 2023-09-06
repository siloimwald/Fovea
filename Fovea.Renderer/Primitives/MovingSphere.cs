using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using static System.Math;

namespace Fovea.Renderer.Primitives
{
    public class MovingSphere : IPrimitive
    {
        private readonly Point3 _center0;
        private readonly Point3 _center1;
        private readonly IMaterial _material;
        private readonly double _radius;
        private readonly double _time0;
        private readonly double _time1;

        public MovingSphere(Point3 center0, double time0, Point3 center1, double time1, double radius,
            IMaterial material)
        {
            _center0 = center0;
            _time0 = time0;
            _center1 = center1;
            _time1 = time1;
            _radius = radius;
            _material = material;
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            var center = CenterAtTime(ray.Time);
            var root = 0.0;
            if (!Sphere.IntersectSphere(ray, center, _radius, rayInterval, ref root))
                return false;

            hitRecord.RayT = root;
            hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT).AsVector3();
            var outwardNormal = (hitRecord.HitPoint - center.AsVector3()) * (1.0f / (float)_radius);
            hitRecord.SetFaceNormal(ray, outwardNormal.AsVec3());
            hitRecord.Material = _material;

            hitRecord.TextureU = 0.5 + Atan2(outwardNormal.X, outwardNormal.Z) / (2 * PI);
            hitRecord.TextureV = outwardNormal.Y * 0.5 + 0.5;

            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return BoundingBox.Union(
                Sphere.SphereBox(CenterAtTime(t0), _radius),
                Sphere.SphereBox(CenterAtTime(t1), _radius));
        }

        private Point3 CenterAtTime(double time)
        {
            return _center0 + (_center1 - _center0) * ((time - _time0) / (_time1 - _time0));
        }
    }
}