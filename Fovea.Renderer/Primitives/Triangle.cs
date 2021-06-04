using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    /// <summary>
    /// first draft for triangle, as a stand-alone object for now.
    /// </summary>
    public class Triangle : IPrimitive
    {
        private readonly Vec3 _edgeAB;
        private readonly Vec3 _edgeAC;
        private readonly IMaterial _material;
        private readonly Vec3 _normal; // precomputed geometric outward normal
        private readonly Point3 _vertexA;

        public Triangle(Point3 vertexA, Point3 vertexB, Point3 vertexC, IMaterial material)
        {
            _vertexA = vertexA;
            _edgeAB = vertexB - vertexA;
            _edgeAC = vertexC - vertexA;
            _normal = Vec3.Normalize(Vec3.Cross(_edgeAB, _edgeAC));
            _material = material;
        }

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var t0 = 0.0;

            if (!TriangleIntersection(ray, _vertexA, _edgeAB, _edgeAC, tMin, tMax, ref t0).HasValue)
                return false;
            
            hitRecord.RayT = t0;
            hitRecord.HitPoint = ray.PointsAt(t0);
            hitRecord.Material = _material;
            hitRecord.SetFaceNormal(ray, _normal);

            return true; // hit at t0
        }

        /// <summary>
        /// triangle intersection, refactored out for reuse in mesh triangle
        /// </summary>
        /// <param name="ray">incoming ray</param>
        /// <param name="vertexA">vertex A of triangle</param>
        /// <param name="edgeAB">edge a to vertex b</param>
        /// <param name="edgeAC">edge a to vertex c</param>
        /// <param name="tMin">ray min t</param>
        /// <param name="tMax">ray max t</param>
        /// <param name="tRay">potential intersection at t</param>
        /// <returns>barycentric coordinates triple if hit, null otherwise</returns>
        public static (double u, double v, double w)? TriangleIntersection(in Ray ray,
                                                in Point3 vertexA,
                                                in Vec3 edgeAB,
                                                in Vec3 edgeAC,
                                                double tMin,
                                                double tMax,
                                                ref double tRay)
        {
            var pVec = Vec3.Cross(ray.Direction, edgeAC);
            var det = Vec3.Dot(edgeAB, pVec);

            if (Math.Abs(det) < 1e-4) // parallel to triangle plane
                return null;

            var invDet = 1.0 / det;

            var tVec = ray.Origin - vertexA;
            var u = Vec3.Dot(tVec, pVec) * invDet;
            if (u is < 0.0f or > 1.0f) return null;
            var qVec = Vec3.Cross(tVec, edgeAB);
            var v = Vec3.Dot(ray.Direction, qVec) * invDet;
            if (v < 0.0f || v + u > 1.0f) return null;
            var t0 = Vec3.Dot(qVec, edgeAC) * invDet;

            if (t0 < tMin || tMax < t0) return null;
            tRay = t0;
            return (u, v, 1.0 - (u + v));
        }
        
        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            var vb = _vertexA + _edgeAB;
            var vc = _vertexA + _edgeAC;
            var min = Point3.Min(Point3.Min(_vertexA, vb), vc);
            var max = Point3.Max(Point3.Max(_vertexA, vb), vc);
            return new BoundingBox(min, max);
        }
    }
}