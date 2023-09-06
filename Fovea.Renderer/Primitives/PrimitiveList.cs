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

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            var hitSomething = false;
            hitRecord.RayT = rayInterval.Max;
            for (var p = 0; p < _primitives.Count; ++p)
                if (_primitives[p].Hit(ray, rayInterval with { Max = hitRecord.RayT }, ref hitRecord))
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

        public float PdfValue(Vector3 origin, Vector3 direction)
        {
            var weight = 1.0f / _primitives.Count;
            return _primitives.Sum(p => weight * p.PdfValue(origin, direction));
        }

        public Vector3 RandomDirection(Vector3 origin)
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