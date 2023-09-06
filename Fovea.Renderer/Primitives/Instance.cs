using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using Fovea.Renderer.VectorMath.Transforms;

namespace Fovea.Renderer.Primitives
{
    /// <summary>
    ///     primitive instancing. Compared to the book, this is the more general approach, but also much more costly. two
    ///     full matrices and a bunch of matrix times vector/point each time we intersect
    /// </summary>
    public class Instance : IPrimitive
    {
        private readonly IPrimitive _instance;
        private readonly Matrix4 _inverseTransform;
        private readonly Matrix4 _transform;

        public Instance(IPrimitive instance, Matrix4 transform, Matrix4 inverseTransform)
        {
            _instance = instance;
            _transform = transform;
            _inverseTransform = inverseTransform;
        }

        public Instance(IPrimitive instance, Transformation transformation)
            : this(instance, transformation.GetMatrix(), transformation.GetInverseMatrix())
        {
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            var transformedRay = new Ray(_inverseTransform * ray.Origin,
                _inverseTransform * ray.Direction);
            if (!_instance.Hit(transformedRay, rayInterval, ref hitRecord))
                return false;

            hitRecord.HitPoint = (_transform * hitRecord.HitPoint.AsPoint3()).AsVector3(); // TODO: yikes.
            // normal needs the transposed of the inverse
            // if a scaling is involved we need to re-normalize
            var n = Vec3.Normalize(_inverseTransform.TransformVectorTransposed(hitRecord.Normal));
            // use untransformed ray here
            hitRecord.SetFaceNormal(ray, n);
            return true;
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            return _instance.GetBoundingBox(t0, t1).Transform(_transform);
        }
    }
}