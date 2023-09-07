using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core.BVH
{
    /// <summary>bin/bucket used during binned SAH construction of BVH Tree</summary>
    public struct SAHBucket
    {
        /// <summary>bounds of this bucket</summary>
        public BoundingBox Bounds;

        /// <summary>primitive in this bucket</summary>
        public int PrimitiveCount;

        // while iterating over the whole scene along one axis, keep track of other information

        public int LeftCount; // accumulated left primitive count
        public int RightCount; // accumulated right primitive count 
        public float LeftArea; // accumulated left area
    }
}