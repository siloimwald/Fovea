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

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            if (!_prim.Hit(ray, tMin, tMax, ref hitRecord))
                return false;

            hitRecord.IsFrontFace = !hitRecord.IsFrontFace;

            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return _prim.GetBoundingBox(t0, t1);
        }

        public double PdfValue(Point3 origin, Vec3 direction)
        {
            return _prim.PdfValue(origin, direction);
        }

        public Vec3 RandomDirection(Point3 origin)
        {
            return _prim.RandomDirection(origin);
        }
    }
}