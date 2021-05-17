using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class PrimitiveList : IPrimitive
    {
        private List<IPrimitive> _primitives = new ();

        public void Add(IPrimitive p) => _primitives.Add(p);
        
        public bool Hit(Ray ray, float tMin, float tMax, HitRecord hitRecord)
        {
            var hitSomething = false;
            hitRecord.RayT = float.PositiveInfinity;
            for (var p = 0; p < _primitives.Count; ++p)
            {
                if (_primitives[p].Hit(ray, tMin, hitRecord.RayT, hitRecord))
                {
                    hitSomething = true;
                }
            }

            // make normal point into right direction
            hitRecord.ProcessNormal(ray);
            
            return hitSomething;
        }
    }
}