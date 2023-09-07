using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Primitives
{
    public class MovingSphere : IPrimitive
    {
        private readonly Vector3 _center0;
        private readonly Vector3 _center1;
        private readonly IMaterial _material;
        private readonly float _radius;
        private readonly float _time0;
        private readonly float _time1;

        public MovingSphere(Vector3 center0, float time0, Vector3 center1, float time1, float radius,
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
            var root = 0.0f;
            if (!Sphere.IntersectSphere(ray, center, _radius, rayInterval, ref root))
                return false;

            hitRecord.RayT = root;
            hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
            var outwardNormal = (hitRecord.HitPoint - center) * (1.0f / _radius);
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = _material;

            hitRecord.TextureU = 0.5f + Atan2(outwardNormal.X, outwardNormal.Z) / (2 * PI);
            hitRecord.TextureV = outwardNormal.Y * 0.5f + 0.5f;

            return true;
        }

        public BoundingBox GetBoundingBox(float t0, float t1)
        {
            return BoundingBox.Union(
                Sphere.SphereBox(CenterAtTime(t0), _radius),
                Sphere.SphereBox(CenterAtTime(t1), _radius));
        }

        private Vector3 CenterAtTime(float time)
        {
            return _center0 + (_center1 - _center0) * ((time - _time0) / (_time1 - _time0));
        }
    }
}