using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core.BVH;

/// <summary>
/// quick and dirty C# variant of the book bvh class. Since we compute Bounding boxes on demand
/// this computes the same boxes over and over again... will improve this
/// </summary>
public class BookBVHNode : IPrimitive
{
    private readonly IPrimitive _left;
    private readonly IPrimitive _right;
    private readonly BoundingBox _box;

    public BookBVHNode(List<IPrimitive> primitives, int start, int end)
    {
        var prims = new List<IPrimitive>(primitives); // shallow copy for sorting
        var nodeSize = end - start;

        var nodeBox = prims.Select(p => p.GetBoundingBox())
            .Aggregate(BoundingBox.CreateMaxEmptyBox(), BoundingBox.Union);
        
        var boxSize = nodeBox.GetExtent();

        var axis = 0;
        if (boxSize.X > boxSize.Y)
        {
            axis = boxSize.X > boxSize.Z ? 0 : 2;
        }
        else
        {
            axis = boxSize.Y > boxSize.Z ? 1 : 2;
        }
        
        if (nodeSize == 1) // single item leaf
        {
            _left = _right = prims[0];
            _box = _left.GetBoundingBox();
        }
        else if (nodeSize == 2) 
        {
            if (Compare(prims[start], prims[start + 1], axis) > 0)
            {
                _left = prims[start];
                _right = prims[start+1];
            }
            else
            {
                _left = prims[start + 1];
                _right = prims[start];
            }
            _box = BoundingBox.Union(_left.GetBoundingBox(), _right.GetBoundingBox());
        }
        else
        {
            // split along the middle
            if (axis == 0)
            {
                prims.Sort(CompareX);
            }
            else if (axis == 1)
            {
                prims.Sort(CompareY);
            }
            else
            {
                prims.Sort(CompareZ);
            }

            var mid = start + nodeSize / 2;
            _left = new BookBVHNode(prims, start, mid);
            _right = new BookBVHNode(prims, mid, end);
            _box = BoundingBox.Union(_left.GetBoundingBox(), _right.GetBoundingBox());
        }
    }
    
    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        if (!_box.Intersect(ray, rayInterval))
            return false;

        var hitLeft = _left.Hit(ray, rayInterval, ref hitRecord);
        var hitRight = _right.Hit(ray, rayInterval with { Max = hitLeft ? hitRecord.RayT : rayInterval.Max },
            ref hitRecord);

        return hitLeft || hitRight;
    }

    public BoundingBox GetBoundingBox()
    {
        return _box;
    }

    private static int CompareX(IPrimitive x, IPrimitive y) => Compare(x, y, 0);
    private static int CompareY(IPrimitive x, IPrimitive y) => Compare(x, y, 1);
    private static int CompareZ(IPrimitive x, IPrimitive y) => Compare(x, y, 2);

    private static int Compare(IPrimitive x, IPrimitive y, int axis)
    {
        var dist = x.GetBoundingBox().Min[axis] - y.GetBoundingBox().Min[axis];
        if (dist < 0)
        {
            return -1;
        }

        return dist > 0 ? 1 : 0;
    }

}

