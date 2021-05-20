using System;
using Fovea.Renderer.Core;
using static System.MathF;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    /// axis aligned bounding box defined by min/max points along the principal axes.
    /// Taken from one of my previous renderer implementations
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// maximal extent of this bounding box
        /// </summary>
        private readonly Point3 _max;

        /// <summary>
        /// minimal extent of this bounding box
        /// </summary>
        private readonly Point3 _min;

        /// <summary>
        /// create a new bounding box with the given min/max. Those are assumed to be correctly ordered already.
        /// </summary>
        /// <param name="min">min. points along all dimensions</param>
        /// <param name="max">max. points along all dimensions</param>
        public BoundingBox(Point3 min, Point3 max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// box center
        /// </summary>
        public Point3 GetCentroid()
        {
            return _min + GetExtent() * 0.5f;
        }

        private Vec3 GetExtent() => _max - _min;

        /// <summary>
        /// compute the volume of bounding box
        /// </summary>
        /// <returns></returns>
        public float GetVolume()
        {
            var ext = GetExtent();
            return ext.X * ext.Y * ext.Z;
        }

        /// <summary>
        /// area of bounding box
        /// </summary>
        /// <returns>area of this bounding box</returns>
        public float GetArea()
        {
            var ext = GetExtent();
            return 2.0f * (ext.X * ext.Y + ext.Y * ext.Z + ext.Z * ext.X);
        }

        /// <summary>
        /// test whether the given ray intersects this bounding box
        /// (Andrew Kensler)
        /// http://psgraphics.blogspot.com/2016/02/new-simple-ray-box-test-from-andrew.html
        /// </summary>
        /// <param name="ray">ray to test against</param>
        /// <param name="tMin">existing min of ray interval</param>
        /// <param name="tMax">existing max of ray interval</param>
        /// <returns>true if ray intersects box</returns>
        public bool Intersect(Ray ray, double tMin, double tMax)
        {
            for (var a = 0; a < 3; ++a)
            {
                var invD = ray.InverseDirection[a];
                var org = ray.Origin[a];
                var t0 = (_min[a] - org) * invD;
                var t1 = (_max[a] - org) * invD;

                if (invD < 0.0f)
                {
                    (t0, t1) = (t1, t0);
                }

                tMin = t0 > tMin ? t0 : tMin;
                tMax = t1 < tMax ? t1 : tMax;
                if (tMax < tMin)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// unite two bounding boxes by computing the minimal box that fully contains both input parameters
        /// </summary>
        /// <param name="boxA">bounding box object</param>
        /// <param name="boxB">bounding box object</param>
        /// <returns>Box = boxA U boxB </returns>
        public static BoundingBox Union(BoundingBox boxA, BoundingBox boxB) 
            => new(Point3.Min(boxA._min, boxB._min), Point3.Max(boxA._max, boxB._max));

        /// <summary>
        /// creates a bounding box with bounds [maxFloat,maxFloat,maxFloat] to [-maxFloat, -maxFloat, -maxFloat]
        /// to be used for union loops
        /// </summary>
        /// <returns>inverted, maximal empty bounding box :)</returns>
        public static BoundingBox CreateMaxEmptyBox()
        {
            return new BoundingBox(
                new Point3(float.MaxValue, float.MaxValue, float.MaxValue),
                new Point3(float.MinValue, float.MinValue, float.MinValue));
        }
        
        /// <summary>
        /// from pbrt book. used for bin projection, given p as a primitive centroid and this as
        /// the centroid bounds of all primitives it scales offsets of primitive centroids from 0,0,0 to 1,1,1
        /// </summary>
        /// <param name="p">centroid of primitive box</param>
        /// <returns></returns>
        public Vec3 Offset(Point3 p)
        {
            var o = p - _min;
            var ext = GetExtent();
            
            // avoid division by zero
            return new Vec3(
                ext.X > 0 ? o.X / ext.X : o.X,
                ext.Y > 0 ? o.Y / ext.Y : o.Y,
                ext.Z > 0 ? o.Z / ext.Z : o.Z);
        }
    }
}