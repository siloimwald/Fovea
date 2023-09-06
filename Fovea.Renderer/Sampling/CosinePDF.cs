using System;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Sampling
{
    public class CosinePDF : IPDF
    {
        private readonly OrthonormalBasis _orthonormalBasis;

        public CosinePDF(OrthonormalBasis orthonormalBasis)
        {
            _orthonormalBasis = orthonormalBasis;
        }

        public float Evaluate(Vector3 direction)
        {
            var cosine = Vector3.Dot(Vector3.Normalize(direction), _orthonormalBasis.WAxis);
            return cosine < 0 ? 0 : cosine / MathF.PI;
        }

        public Vector3 Generate()
        {
            
            return _orthonormalBasis.Local(Sampler.Instance.RandomCosineDirection().AsVector3());
        }
    }
}