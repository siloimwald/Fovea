using System.Collections.Generic;
using System.Linq;

namespace Fovea.Renderer.Parser.Descriptors.Transforms;

public static class TransformationListExtensions
{
    public static Matrix4x4 GetTransformation(this IEnumerable<ITransformationDescriptor> transforms)
    {
        return transforms.Aggregate(Matrix4x4.Identity,
            (acc, transform) => acc * transform.GetTransformationMatrix());
    }
}