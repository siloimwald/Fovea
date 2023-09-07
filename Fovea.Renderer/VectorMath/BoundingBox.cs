using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Fovea.Renderer.Core;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    ///     axis aligned bounding box defined by min/max points along the principal axes. Taken from one of my previous
    ///     renderer implementations
    /// </summary>
    public class BoundingBox
    {
        private const byte ShuffleMask = (3 << 4) | (2 << 2) | 1;

        /// <summary>maximal extent of this bounding box</summary>
        private readonly Vector3 _max;

        /// <summary>minimal extent of this bounding box</summary>
        private readonly Vector3 _min;

        /// <summary>create a new bounding box with the given min/max. Those are assumed to be correctly ordered already.</summary>
        /// <param name="min">min. points along all dimensions</param>
        /// <param name="max">max. points along all dimensions</param>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>box center</summary>
        public Vector3 GetCentroid()
        {
            return _min + GetExtent() * 0.5f;
        }

        public Vector3 GetExtent()
        {
            return _max - _min;
        }

        /// <summary>compute the volume of bounding box</summary>
        /// <returns></returns>
        public float GetVolume()
        {
            var ext = GetExtent();
            return ext.X * ext.Y * ext.Z;
        }

        /// <summary>area of bounding box</summary>
        /// <returns>area of this bounding box</returns>
        public float GetArea()
        {
            var ext = GetExtent();
            return 2.0f * (ext.X * ext.Y + ext.Y * ext.Z + ext.Z * ext.X);
        }

        /// <summary>test whether the given ray intersects this bounding box</summary>
        /// <param name="ray">ray to test against</param>
        /// <param name="tMin">existing min of ray interval</param>
        /// <param name="tMax">existing max of ray interval</param>
        /// <returns>true if ray intersects box</returns>
        public bool Intersect(in Ray ray, float tMin, float tMax)
        {
            var tx1 = (_min.X - ray.Origin.X) * ray.InverseDirection.X;
            var tx2 = (_max.X - ray.Origin.X) * ray.InverseDirection.X;

            tMin = Math.Max(tMin, Math.Min(tx1, tx2));
            tMax = Math.Min(tMax, Math.Max(tx1, tx2));

            var ty1 = (_min.Y - ray.Origin.Y) * ray.InverseDirection.Y;
            var ty2 = (_max.Y - ray.Origin.Y) * ray.InverseDirection.Y;

            tMin = Math.Max(tMin, Math.Min(ty1, ty2));
            tMax = Math.Min(tMax, Math.Max(ty1, ty2));

            var tz1 = (_min.Z - ray.Origin.Z) * ray.InverseDirection.Z;
            var tz2 = (_max.Z - ray.Origin.Z) * ray.InverseDirection.Z;

            tMin = Math.Max(tMin, Math.Min(tz1, tz2));
            tMax = Math.Min(tMax, Math.Max(tz1, tz2));

            return tMax >= tMin && tMax >= 0.0;
        }

        public bool IntersectSse(in Ray ray, float tMin, float tMax)
        {
            var invDir = Vector128.Create(ray.InverseDirection.X, ray.InverseDirection.Y,
                ray.InverseDirection.Z, 0.0f);
            var org = Vector128.Create(ray.Origin.X, ray.Origin.Y, ray.Origin.Z, 0.0f);
            var minVec = Vector128.Create(_min.X, _min.Y, _min.Z, 0.0f);
            var maxVec = Vector128.Create(_max.X, _max.Y, _max.Z, 0.0f);

            var t0 = Sse.Multiply(Sse.Subtract(minVec, org), invDir);
            var t1 = Sse.Multiply(Sse.Subtract(maxVec, org), invDir);

            var min = Sse.Min(t0, t1);
            var max = Sse.Max(t0, t1);

            // compares min0 and min1 and min1 and min2
            // shuffle again and compare to get overall min/max in first component
            var minStage0 = Sse.Max(Sse.Shuffle(min, min, ShuffleMask), min);
            var gTMin = Sse.Max(Sse.Shuffle(minStage0, minStage0, ShuffleMask), minStage0).GetElement(0);
            var maxStage0 = Sse.Min(Sse.Shuffle(max, max, ShuffleMask), max);
            var gTMax = Sse.Min(Sse.Shuffle(maxStage0, maxStage0, ShuffleMask), maxStage0).GetElement(0);

            gTMin = Math.Max(tMin, gTMin);
            gTMax = Math.Min(tMax, gTMax);
            return gTMax >= gTMin && gTMax > 0;
        }

        /// <summary>unite two bounding boxes by computing the minimal box that fully contains both input parameters</summary>
        /// <param name="boxA">bounding box object</param>
        /// <param name="boxB">bounding box object</param>
        /// <returns>Box = boxA U boxB </returns>
        public static BoundingBox Union(BoundingBox boxA, BoundingBox boxB)
        {
            return new(Vector3.Min(boxA._min, boxB._min), Vector3.Max(boxA._max, boxB._max));
        }

        /// <summary>compute the intersection of two bounding boxes</summary>
        /// <param name="boxA">bounding box object</param>
        /// <param name="boxB">bounding box object</param>
        /// <returns>box = boxA n BoxB</returns>
        public static BoundingBox Intersect(BoundingBox boxA, BoundingBox boxB)
        {
            return new(
                Vector3.Max(boxA._min, boxB._min),
                Vector3.Min(boxA._max, boxB._max));
        }

        /// <summary>
        ///     creates a bounding box with bounds [maxFloat,maxFloat,maxFloat] to [-maxFloat, -maxFloat, -maxFloat] to be
        ///     used for union loops
        /// </summary>
        /// <returns>inverted, maximal empty bounding box :)</returns>
        public static BoundingBox CreateMaxEmptyBox()
        {
            return new(
                new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                new Vector3(float.MinValue, float.MinValue, float.MinValue));
        }

        /// <summary>
        ///     from pbrt book. used for bin projection, given p as a primitive centroid and this as the centroid bounds of
        ///     all primitives it scales offsets of primitive centroids from 0,0,0 to 1,1,1
        /// </summary>
        /// <param name="p">centroid of primitive box</param>
        /// <returns></returns>
        public Vector3 Offset(Vector3 p)
        {
            var o = p - _min;
            var ext = GetExtent();

            // avoid division by zero
            return new Vector3(
                ext.X > 0 ? o.X / ext.X : o.X,
                ext.Y > 0 ? o.Y / ext.Y : o.Y,
                ext.Z > 0 ? o.Z / ext.Z : o.Z);
        }

        public BoundingBox Transform(Matrix4x4 transform)
        {
            var transformedMin = Vector3.Transform(_min, transform);
            var transformedMax = Vector3.Transform(_max, transform);
            return new BoundingBox(
                Vector3.Min(transformedMin, transformedMax),
                Vector3.Max(transformedMin, transformedMax));
        }
    }
}