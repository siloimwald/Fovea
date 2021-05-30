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
        private readonly double _negInvDensity;
        private readonly Isotropic _material;
        
        public ConstantMedium(IPrimitive boundary, double density, ITexture color)
        {
            _negInvDensity = -1.0 / density;
            _boundary = boundary;
            _material = new Isotropic(color);
        }
        
        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var hr1 = new HitRecord();
            var hr2 = new HitRecord();

            if (!_boundary.Hit(ray, double.NegativeInfinity, double.PositiveInfinity, ref hr1))
                return false;

            if (!_boundary.Hit(ray, hr1.RayT + 0.001, double.PositiveInfinity, ref hr2))
                return false;

            hr1.RayT = Math.Max(hr1.RayT, tMin);
            hr2.RayT = Math.Min(hr2.RayT, tMax);

            if (hr1.RayT >= hr2.RayT)
                return false;

            if (hr1.RayT < 0)
                hr1.RayT = 0;

            var rayLength = ray.Direction.Length();
            var distanceInsideBoundary = (hr2.RayT - hr1.RayT) * rayLength;
            var hitDistance = _negInvDensity * Math.Log(Sampler.Instance.Random01());

            if (hitDistance > distanceInsideBoundary)
                return false;

            hitRecord.RayT = hr1.RayT + hitDistance / rayLength;
            hitRecord.HitPoint = ray.PointsAt(hitRecord.RayT);
            hitRecord.Material = _material;
            
            // front face/normal left unset, never used due to fixed material

            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return _boundary.GetBoundingBox(t0, t1);
        }
    }
}