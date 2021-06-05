using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class PrimitiveList : IPrimitive
    {
        private readonly List<IPrimitive> _primitives;

        public PrimitiveList(List<IPrimitive> prims)
        {
            _primitives = prims;
        }

        public PrimitiveList()
        {
            _primitives = new List<IPrimitive>();
        }

        public IPrimitive this[int index] => _primitives[index];

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var hitSomething = false;
            hitRecord.RayT = tMax;
            for (var p = 0; p < _primitives.Count; ++p)
                if (_primitives[p].Hit(ray, tMin, hitRecord.RayT, ref hitRecord))
                    hitSomething = true;

            return hitSomething;
        }


        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return
                _primitives
                    .Select(p => p.GetBoundingBox(t0, t1))
                    .Aggregate(BoundingBox.CreateMaxEmptyBox(), BoundingBox.Union);
        }

        public double PdfValue(Point3 origin, Vec3 direction)
        {
            var weight = 1.0 / _primitives.Count;
            return _primitives.Sum(p => weight * p.PdfValue(origin, direction));
        }

        public Vec3 RandomDirection(Point3 origin)
        {
            var pIndex = Sampler.Instance.RandomInt(0, _primitives.Count);
            return _primitives[pIndex].RandomDirection(origin);
        }

        public void Add(IPrimitive p)
        {
            _primitives.Add(p);
        }
    }
}