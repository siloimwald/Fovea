using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Sampling
{
    public class MixturePDF : IPDF
    {
        private readonly IPDF _left;
        private readonly IPDF _right;

        public MixturePDF(IPDF left, IPDF right)
        {
            _left = left;
            _right = right;
        }

        public double Evaluate(Vec3 direction)
        {
            return 0.5 * _left.Evaluate(direction) + 0.5 * _right.Evaluate(direction);
        }

        public Vec3 Generate()
        {
            return Sampler.Instance.Random01() < 0.5 ? _left.Generate() : _right.Generate();
        }
    }
}