using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class FlipFace : IPrimitive
    {
        private readonly IPrimitive _prim;

        public FlipFace(IPrimitive prim)
        {
            _prim = prim;
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            if (!_prim.Hit(ray, rayInterval, ref hitRecord))
                return false;

            hitRecord.IsFrontFace = !hitRecord.IsFrontFace;

            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return _prim.GetBoundingBox(t0, t1);
        }

        public float PdfValue(Vector3 origin, Vector3 direction)
        {
            return _prim.PdfValue(origin, direction);
        }

        public Vector3 RandomDirection(Vector3 origin)
        {
            return _prim.RandomDirection(origin);
        }
    }
}