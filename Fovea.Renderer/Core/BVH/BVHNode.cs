using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core.BVH;

public class BVHNode
{
    /// <summary>bounding box of node</summary>
    public BoundingBox Box;

    /// <summary>primitive count for leaf node, 0 zero for inner node</summary>
    public int Count;

    /// <summary>
    ///     index of right child, left child is own position in array + 1. For leaves this is the start into the primitive
    ///     array
    /// </summary>
    public int OtherNodeFirstPrim;
}