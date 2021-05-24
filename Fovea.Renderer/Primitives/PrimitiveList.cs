using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
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

        public void Add(IPrimitive p) => _primitives.Add(p);
        
        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var hitSomething = false;
            hitRecord.RayT = tMax;
            for (var p = 0; p < _primitives.Count; ++p)
            {
                if (_primitives[p].Hit(ray, tMin, hitRecord.RayT, ref hitRecord))
                {
                    hitSomething = true;
                }
            }

            return hitSomething;
        }

        public BoundingBox GetBoundingBox()
        {
            return 
                _primitives
                    .Select(p => p.GetBoundingBox())
                    .Aggregate(BoundingBox.CreateMaxEmptyBox(), BoundingBox.Union);
        }
    }
}