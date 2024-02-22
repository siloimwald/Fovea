using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

/// <summary>
///     primitive instancing. Compared to the book, this is the more general approach, but also much more costly. two
///     full matrices and a bunch of matrix times vector/point each time we intersect
/// </summary>
public class Instance : IPrimitive
{
    private readonly IPrimitive _blueprint;
    private readonly IMaterial _material;
    private readonly bool _useParentMaterial;
    private readonly Matrix4x4 _inverseTransform;
    private readonly Matrix4x4 _transform;
    private readonly Matrix4x4 _normalTransform;

    public Instance(IPrimitive blueprint, 
                    Matrix4x4 transform, 
                    Matrix4x4 inverseTransform,
                    IMaterial material,
                    bool useParentMaterial = false)
    {
        _blueprint = blueprint;
        _transform = transform;
        _inverseTransform = inverseTransform;
        _normalTransform = Matrix4x4.Transpose(inverseTransform);
        _material = material;
        _useParentMaterial = useParentMaterial;
    }

    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
            
        var inverseOrg = Vector3.Transform(ray.Origin, _inverseTransform);
        // judging from the code, TransformNormal should be named transform direction... i guess
        var inverseDir = Vector3.TransformNormal(ray.Direction, _inverseTransform);
        
        var transformedRay = new Ray(inverseOrg, inverseDir);
            
        if (!_blueprint.Hit(transformedRay, rayInterval, ref hitRecord))
            return false;

        hitRecord.HitPoint = Vector3.Transform(hitRecord.HitPoint, _transform);
        
        if (!_useParentMaterial)
        {
            hitRecord.Material = _material;
        }

        // normal needs the transposed of the inverse
        // if a scaling is involved we need to re-normalize?
        var n = Vector3.Normalize(Vector3.TransformNormal(hitRecord.Normal, _normalTransform));
        // use untransformed ray here
        hitRecord.SetFaceNormal(ray, n);
        return true;
    }

    public BoundingBox GetBoundingBox()
    {
        return _blueprint.GetBoundingBox().Transform(_transform);
    }
}