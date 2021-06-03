using System;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Sampling
{
    public class CosinePDF : IPDF
    {
        private readonly OrthoNormalBasis _orthoNormalBasis;

        public CosinePDF(OrthoNormalBasis orthoNormalBasis)
        {
            _orthoNormalBasis = orthoNormalBasis;
        }
        
        public double Evaluate(Vec3 direction)
        {
            var cosine = Vec3.Dot(direction, _orthoNormalBasis.WAxis);
            return cosine < 0 ? 0 : cosine / Math.PI;
        }

        public Vec3 Generate()
        {
            return _orthoNormalBasis.Local(Sampler.Instance.RandomCosineDirection());
        }
    }
}