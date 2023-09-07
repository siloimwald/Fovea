using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class ConstantMedium : IPrimitive
    {
        private readonly IPrimitive _boundary;
        private readonly Isotropic _material;
        private readonly float _negInvDensity;

        public ConstantMedium(IPrimitive boundary, float density, ITexture color)
        {
            _negInvDensity = -1.0f / density;
            _boundary = boundary;
            _material = new Isotropic(color);
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            var hr1 = new HitRecord();
            var hr2 = new HitRecord();

            if (!_boundary.Hit(ray, Interval.Everything(), ref hr1))
                return false;

            if (!_boundary.Hit(ray, Interval.HalfOpenWithOffset() with { Min = hr1.RayT + 0.001f}, ref hr2))
                return false;

            hr1.RayT = Math.Max(hr1.RayT, rayInterval.Min);
            hr2.RayT = Math.Min(hr2.RayT, rayInterval.Max);

            if (hr1.RayT >= hr2.RayT)
                return false;

            if (hr1.RayT < 0)
                hr1.RayT = 0;

            var rayLength = (float)ray.Direction.Length();
            var distanceInsideBoundary = (hr2.RayT - hr1.RayT) * rayLength;
            var hitDistance = _negInvDensity * MathF.Log((float)Sampler.Instance.Random01());

            if (hitDistance > distanceInsideBoundary)
                return false;

            hitRecord.RayT = hr1.RayT + hitDistance / rayLength;
            hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
            hitRecord.Material = _material;

            // front face/normal left unset, never used due to fixed material

            return true;
        }

        public BoundingBox GetBoundingBox(float t0, float t1)
        {
            return _boundary.GetBoundingBox(t0, t1);
        }
    }
}