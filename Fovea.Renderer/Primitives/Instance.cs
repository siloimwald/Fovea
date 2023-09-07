using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    /// <summary>
    ///     primitive instancing. Compared to the book, this is the more general approach, but also much more costly. two
    ///     full matrices and a bunch of matrix times vector/point each time we intersect
    /// </summary>
    public class Instance : IPrimitive
    {
        private readonly IPrimitive _instance;
        private readonly Matrix4x4 _inverseTransform;
        private readonly Matrix4x4 _transform;

        public Instance(IPrimitive instance, Matrix4x4 transform, Matrix4x4 inverseTransform)
        {
            _instance = instance;
            _transform = transform;
            _inverseTransform = inverseTransform;
        }

        public Instance(IPrimitive instance, Transform transformation)
            
        {
            var (forward, inverse, _) = transformation.Build();
            _instance = instance;
            _transform = forward;
            _inverseTransform = inverse;
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            
            var inverseOrg = Vector3.Transform(ray.Origin, _inverseTransform);
            var inverseDir = Vector3.Transform(ray.Direction, _inverseTransform);
            
            var transformedRay = new Ray(inverseOrg, inverseDir);
            
            if (!_instance.Hit(transformedRay, rayInterval, ref hitRecord))
                return false;

            hitRecord.HitPoint = Vector3.Transform(hitRecord.HitPoint, _transform);
            // normal needs the transposed of the inverse
            // if a scaling is involved we need to re-normalize
            var n = Vector3.Normalize(Vector3.TransformNormal(hitRecord.Normal, _inverseTransform));
            // use untransformed ray here
            hitRecord.SetFaceNormal(ray, n);
            return true;
        }

        public BoundingBox GetBoundingBox(float t0, float t1)
        {
            return _instance.GetBoundingBox(t0, t1).Transform(_transform);
        }
    }
}