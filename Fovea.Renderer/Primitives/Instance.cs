using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    /// <summary>
    /// primitive instancing.
    /// Compared to the book, this is the more general approach, but also much more costly.
    /// two full matrices and a bunch of matrix times vector/point each time we intersect
    /// </summary>
    public class Instance : IPrimitive
    {
        private readonly IPrimitive _instance;
        private readonly Matrix4 _transform;
        private readonly Matrix4 _inverseTransform;

        public Instance(IPrimitive instance, Matrix4 transform, Matrix4 inverseTransform)
        {
            _instance = instance;
            _transform = transform;
            _inverseTransform = inverseTransform;
        }
        
        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var transformedRay = new Ray(_inverseTransform * ray.Origin,
                _inverseTransform * ray.Direction);
            if (!_instance.Hit(transformedRay, tMin, tMax, ref hitRecord))
                return false;

            hitRecord.HitPoint = _transform * hitRecord.HitPoint;
            // normal needs the transposed of the inverse
            // if a scaling is involved we need to re-normalize
            var n = Vec3.Normalize(_inverseTransform.TransformVectorTransposed(hitRecord.Normal));
            // use untransformed ray here
            hitRecord.SetFaceNormal(ray, n);
            return true;
        }

        public BoundingBox GetBoundingBox()
        {
            return _instance.GetBoundingBox().Transform(_transform);
        }
    }
}