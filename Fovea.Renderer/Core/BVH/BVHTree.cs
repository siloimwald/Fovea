using System;
using System.Collections.Generic;
using Fovea.Renderer.Extensions;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core.BVH
{
    /// <summary>
    /// BVH implementation from a previous fovea iteration. This is based on - as far as i can remember -
    /// an implementation given in the PBRT book. It uses a sah cost based approach, but bins primitives
    /// instead of doing a full evaluation
    /// </summary>
    public class BVHTree : IPrimitive
    {
        /// <summary>
        /// max recursion/tree depth
        /// </summary>
        private const int MaxDepth = 32;

        /// <summary>
        /// buckets/bins to use during sah evaluation
        /// </summary>
        private const int BucketCount = 128;

        /// <summary>
        /// create a leaf if a node's centroid bounds volume is below this value
        /// </summary>
        private const double BoundsVolumeThreshold = 1e-4;

        /// <summary>
        /// minimal primitive count in a leaf node before stopping recursion
        /// </summary>
        private const int MinPrimCount = 2;

        private readonly BVHNode[] _nodes;

        private readonly List<IPrimitive> _primitives;
        private int _nodeIndex;

        /// <summary>
        /// build a bvh structure for the given primitives using 'sah binning' as a splitting
        /// strategy
        /// </summary>
        /// <param name="primitives">list of scene primitives</param>
        public BVHTree(List<IPrimitive> primitives)
        {
            _nodes = new BVHNode[2 * primitives.Count - 1];
            var primitiveBoxes = new List<BoundingBox>(primitives.Count);
            _primitives = primitives;

            var sceneBounds = BoundingBox.CreateMaxEmptyBox();
            // precompute bounding boxes for all primitives and compute scene bounds along the way
            foreach (var t in primitives)
            {
                var primBox = t.GetBoundingBox();
                primitiveBoxes.Add(primBox);
                sceneBounds = BoundingBox.Union(sceneBounds, primBox);
            }

            // hard work here
            BuildRec(sceneBounds, 0, primitives.Count, 0, primitiveBoxes);

            // shrink node array to what we actually used
            Array.Resize(ref _nodes, _nodeIndex);

            // print out some stats...

            var maxLeafSize = 0;
            var primCount = 0;
            var leafCount = 0;
            var inner = 0;

            var s = new Stack<int>();
            s.Push(0);
            while (s.Count > 0)
            {
                var nIndex = s.Pop();
                var node = _nodes[nIndex];
                if (node.Count == 0) // inner node
                {
                    inner++;
                    s.Push(nIndex + 1);
                    s.Push(node.OtherNodeFirstPrim);
                }
                else
                {
                    primCount += node.Count;
                    maxLeafSize = Math.Max(maxLeafSize, node.Count);
                    leafCount++;
                }
            }

            Console.WriteLine("*** tree stats ***");
            Console.WriteLine(
                $"Primitive count = {primCount}"); // more a self check, should exactly match the scene primitive count since we don't duplicate things
            Console.WriteLine(
                $"Inner Nodes {inner}, Leaf nodes {leafCount}, max leaf size {maxLeafSize}, total node count {_nodes.Length}");
        }

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            Span<int> nodeStack = stackalloc int[MaxDepth * 2];
            
            var pointer = 0;
            nodeStack[pointer++] = 0;

            var hit = false;
            hitRecord.RayT = tMax;
            
            while (pointer > 0)
            {
                var nodeIndex = nodeStack[--pointer];
                var node = _nodes[nodeIndex];

                if (!node.Box.Intersect(ray, 0, hitRecord.RayT))
                    continue;

                if (node.Count > 0)
                {
                    for (var p = node.OtherNodeFirstPrim; p < node.OtherNodeFirstPrim + node.Count; ++p)
                    {
                        if (!_primitives[p].Hit(ray, tMin, hitRecord.RayT, ref hitRecord)) continue;
                        hit = true;
                    }
                }
                else
                {
                    nodeStack[pointer++] = nodeIndex + 1;
                    nodeStack[pointer++] = node.OtherNodeFirstPrim;
                }
            }

            // make normal point into right direction
            if (hit)
            {
                hitRecord.ProcessNormal(ray);
            }
            
            return hit;
        }

        public BoundingBox GetBoundingBox()
        {
            return _nodes[0]?.Box; // root node box
        }

        /// <summary>
        /// compute the best split position for the current node
        /// </summary>
        /// <param name="centroidBounds">centroid bounds of current node</param>
        /// <param name="totalArea">area of that node</param>
        /// <param name="left">left index into primitive array</param>
        /// <param name="right">right index into primitive array</param>
        /// <param name="primitiveBoxes">list of precomputed bounding boxes of primitives</param>
        /// <returns></returns>
        private static (int axis, int bucket, double costs) GetBestSplit(
            BoundingBox centroidBounds,
            double totalArea,
            int left,
            int right,
            IReadOnlyList<BoundingBox> primitiveBoxes)
        {
            var bestBucket = -1;
            var bestCosts = double.MaxValue;
            var bestAxis = -1;

            for (var axis = 0; axis < 3; ++axis)
            {
                // prepare bins/buckets for this dimension
                var buckets = new SAHBucket[BucketCount];
                for (var b = 0; b < buckets.Length; ++b)
                {
                    buckets[b] = new SAHBucket
                    {
                        Bounds = BoundingBox.CreateMaxEmptyBox(),
                        PrimitiveCount = 0
                    };
                }

                // project primitives into their bucket
                for (var p = left; p < right; ++p)
                {
                    var binId = GetBinProjection(primitiveBoxes[p].GetCentroid(), centroidBounds, axis);
                    buckets[binId].PrimitiveCount++;
                    buckets[binId].Bounds = BoundingBox.Union(buckets[binId].Bounds, primitiveBoxes[p]);
                }

                var leftSweepBox = BoundingBox.CreateMaxEmptyBox();
                var rightSweepBox = BoundingBox.CreateMaxEmptyBox();

                // sweep left to right, accumulate primitive counts and boxes as we go
                for (var b = 0; b < BucketCount; ++b)
                {
                    leftSweepBox = BoundingBox.Union(leftSweepBox, buckets[b].Bounds);
                    buckets[b].LeftArea = leftSweepBox.GetArea();
                    buckets[b].LeftCount = buckets[b].PrimitiveCount + (b > 0 ? buckets[b - 1].LeftCount : 0);
                }

                // ...and right to left
                for (var b = BucketCount - 1; b >= 0; --b)
                {
                    rightSweepBox = BoundingBox.Union(rightSweepBox, buckets[b].Bounds);
                    var rightArea = rightSweepBox.GetArea();
                    buckets[b].RightCount = buckets[b].PrimitiveCount +
                                            ((b < BucketCount - 1) ? buckets[b + 1].RightCount : 0);

                    var costs = ComputeSAH(totalArea, buckets[b].LeftArea, buckets[b].LeftCount,
                        rightArea, buckets[b].RightCount);

                    if (!(costs < bestCosts)) continue;
                    bestCosts = costs;
                    bestBucket = b;
                    bestAxis = axis;
                }
            }

            return (bestAxis, bestBucket, bestCosts);
        }

        /// <summary>
        /// build a bvh structure for the given node
        /// </summary>
        /// <param name="nodeBox">bounding box of node to work on</param>
        /// <param name="left">left index into primitive array</param>
        /// <param name="right">right index into primitive array</param>
        /// <param name="depth">current tree depth</param>
        /// <param name="primitiveBoxes">precomputed list of bounding boxes for primitives</param>
        private void BuildRec(BoundingBox nodeBox, int left, int right, int depth, List<BoundingBox> primitiveBoxes)
        {
            var count = right - left;
            if (depth >= MaxDepth || count <= MinPrimCount) // reach termination criteria, make a leaf node
            {
                _nodes[_nodeIndex++] = MakeLeaf(nodeBox, left, count);
            }
            else
            {
                // compute centroid bounds for current node box
                var (centroidMin, centroidMax) = (new Point3(double.MaxValue), new Point3(double.MinValue));
                for (var p = left; p < right; ++p)
                {
                    centroidMin = Point3.Min(centroidMin, primitiveBoxes[p].GetCentroid());
                    centroidMax = Point3.Max(centroidMax, primitiveBoxes[p].GetCentroid());
                }

                var centroidBounds = new BoundingBox(centroidMin, centroidMax);

                // node volume too small to do any meaningful splitting, make a leaf node
                if (centroidBounds.GetVolume() < BoundsVolumeThreshold)
                {
                    _nodes[_nodeIndex++] = MakeLeaf(nodeBox, left, count);
                }
                else
                {
                    var nodeArea = nodeBox.GetArea();
                    var (axis, bucket, costs) = GetBestSplit(centroidBounds, nodeArea, left, right, primitiveBoxes);

                    // splitting is better than doing a leaf (this assumes C_(traversal/intersect) to be 1)
                    if (costs < count)
                    {
                        var leftBox = BoundingBox.CreateMaxEmptyBox();
                        var rightBox = BoundingBox.CreateMaxEmptyBox();

                        if (bucket == BucketCount - 1)
                            bucket--;

                        var lc = left; // first index to sort 'left' primitives into

                        // classify according to best split
                        // basically one pass of insertion sort, everything that goes left is inserted to the left end of the list
                        // pushing the right-side primitives to the right
                        for (var p = left; p < right; ++p)
                        {
                            var primBox = primitiveBoxes[p];
                            var bucketId = GetBinProjection(primBox.GetCentroid(), centroidBounds, axis);

                            if (bucketId <= bucket)
                            {
                                if (p != lc)
                                {
                                    _primitives.SwapElements(p, lc);
                                    primitiveBoxes.SwapElements(p, lc);
                                }

                                lc++;
                                leftBox = BoundingBox.Union(leftBox, primBox);
                            }
                            else
                            {
                                rightBox = BoundingBox.Union(rightBox, primBox);
                            }
                        }

                        var middle = lc;

                        // make inner node
                        var node = _nodes[_nodeIndex++] = new BVHNode
                        {
                            Box = nodeBox
                        };

                        BuildRec(leftBox, left, middle, depth + 1, primitiveBoxes);
                        node.OtherNodeFirstPrim = _nodeIndex;
                        BuildRec(rightBox, middle, right, depth + 1, primitiveBoxes);
                    }
                    else
                    {
                        _nodes[_nodeIndex++] = MakeLeaf(nodeBox, left, count);
                    }
                }
            }
        }


        /// <summary>
        /// project a bounding box into one of the sah bins/buckets
        /// </summary>
        /// <param name="boxCentroid">bounding box centroid'</param>
        /// <param name="centroidBounds">centroid bounds of current node</param>
        /// <param name="axis">axis we're working on</param>
        /// <returns>int [0..BucketCount-1]</returns>
        private static int GetBinProjection(Point3 boxCentroid, BoundingBox centroidBounds, int axis)
        {
            var b = (int) (BucketCount * centroidBounds.Offset(boxCentroid)[axis]);
            if (b == BucketCount) b--;
            return b;
        }
        
        /// <summary>
        /// compute the surface area heuristic costs scheme for the propose split
        /// </summary>
        /// <param name="totalArea">area of parent node</param>
        /// <param name="leftArea">area left of split</param>
        /// <param name="leftPrimitiveCount">primitive count in left part of split</param>
        /// <param name="rightArea">area right of split</param>
        /// <param name="rightPrimitiveCount">primitive count in right part of split</param>
        /// <returns></returns>
        private static double ComputeSAH(double totalArea,
            double leftArea,
            int leftPrimitiveCount,
            double rightArea,
            int rightPrimitiveCount)
        {
            // the costs for intersection and traversal are assumed to be constant and are omitted
            return leftArea / totalArea * leftPrimitiveCount +
                   rightArea / totalArea * rightPrimitiveCount;
        }

        private static BVHNode MakeLeaf(BoundingBox leafBox, int firstPrim, int count)
        {
            return new()
            {
                Count = count,
                OtherNodeFirstPrim = firstPrim,
                Box = leafBox
            };
        }
    }
}