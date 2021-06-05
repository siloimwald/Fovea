using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Sampling
{
    public class PrimitivePDF : IPDF
    {
        private readonly Point3 _origin;
        private readonly IPrimitive _primitive;

        public PrimitivePDF(IPrimitive primitive, Point3 origin)
        {
            _primitive = primitive;
            _origin = origin;
        }

        public double Evaluate(Vec3 direction)
        {
            return _primitive.PdfValue(_origin, direction);
        }

        public Vec3 Generate()
        {
            return _primitive.RandomDirection(_origin);
        }
    }
}