using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class Cylinder : IPrimitive
    {
        private readonly IMaterial _material;
        private readonly double _radius;
        private readonly double _zMax;
        private readonly double _zMin;

        public Cylinder(double zMin, double zMax, double radius, IMaterial material)
        {
            _zMin = Math.Min(zMin, zMax);
            _zMax = Math.Max(zMin, zMax);
            _radius = radius;
            _material = material;
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            // get better cap hit, if any
            var tCap = 0.0;
            var hitCap = TestCapHit(ray, rayInterval, ref tCap);
            var tBody = 0.0;
            var hitBody = TestBodyHit(ray, rayInterval, ref tBody);

            if (!hitBody && !hitCap)
                return false;

            if (hitBody && !hitCap || hitBody && tBody < tCap)
            {
                var hitPoint = ray.PointsAt(tBody);
                var n = (hitPoint - new Point3(0, 0, hitPoint.PZ)) * (1.0 / _radius);
                hitRecord.HitPoint = hitPoint;
                hitRecord.Material = _material;
                hitRecord.RayT = tBody;

                var theta = Math.Atan2(n.X, n.Y);
                hitRecord.TextureV = 0.5 + theta / (2 * Math.PI);
                hitRecord.TextureU = (hitPoint.PZ - _zMin) / (_zMax - _zMin);

                hitRecord.SetFaceNormal(ray, n);
                return true;
            }


            // cap hit
            hitRecord.HitPoint = ray.PointsAt(tCap);
            hitRecord.RayT = tCap;
            hitRecord.Material = _material;

            hitRecord.TextureU = 0.5 + hitRecord.HitPoint.PX / _radius * 0.5;
            hitRecord.TextureV = 0.5 + hitRecord.HitPoint.PY / _radius * 0.5;

            // flip the normal accordingly
            var s = hitRecord.HitPoint.PZ < 0 ? -1 : 1;
            hitRecord.SetFaceNormal(ray, new Vec3(0, 0, s));
            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            var min = new Point3(-_radius, -_radius, _zMin);
            var max = new Point3(_radius, _radius, _zMax);
            return new BoundingBox(min, max);
        }

        /// <summary>returns true if we hit either of the caps</summary>
        /// <param name="ray">ray</param>
        /// <param name="rayInterval">ray interval</param>
        /// <param name="tCap">reference to better cap hit</param>
        /// <returns></returns>
        private bool TestCapHit(in Ray ray, in Interval rayInterval, ref double tCap)
        {
            // parallel to plane
            if (Math.Abs(ray.Direction.Z) < 1e-6)
                return false;

            var t0 = (_zMin - ray.Origin.PZ) / ray.Direction.Z;
            var t1 = (_zMax - ray.Origin.PZ) / ray.Direction.Z;

            if (t0 > t1) MathUtils.Swap(ref t0, ref t1);

            tCap = t0;
            var hp = ray.PointsAt(tCap);
            var r2 = _radius * _radius;
            if (rayInterval.Contains(tCap) && !(hp.PX * hp.PX + hp.PY * hp.PY > r2)) return true;
            tCap = t1;
            hp = ray.PointsAt(tCap);

            return rayInterval.Contains(tCap) && !(hp.PX * hp.PX + hp.PY * hp.PY > r2);
        }

        /// <summary>test ray against cylinder body</summary>
        /// <param name="ray">incoming ray</param>
        /// <param name="rayInterval">ray interval</param>
        /// <param name="tBody">potential closest body hit</param>
        /// <returns></returns>
        private bool TestBodyHit(in Ray ray, in Interval rayInterval, ref double tBody)
        {
            var a = ray.Direction.X * ray.Direction.X + ray.Direction.Y * ray.Direction.Y;
            var b = 2.0 * (ray.Direction.X * ray.Origin.PX + ray.Direction.Y * ray.Origin.PY);
            var c = ray.Origin.PX * ray.Origin.PX + ray.Origin.PY * ray.Origin.PY - _radius * _radius;

            double t0 = 0.0, t1 = 0.0;
            if (!MathUtils.SolveQuadratic(a, b, c, ref t0, ref t1))
                return false;

            tBody = t0;
            // test against ray interval and clip
            var hpz = ray.Origin.PZ + tBody * ray.Direction.Z;
            if (!rayInterval.Contains(tBody) || hpz < _zMin || hpz > _zMax)
            {
                tBody = t1;
                hpz = ray.Origin.PZ + tBody * ray.Direction.Z;
                if (!rayInterval.Contains(tBody) || hpz < _zMin || hpz > _zMax)
                    return false;
            }

            return true;
        }
    }
}