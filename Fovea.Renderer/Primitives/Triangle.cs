using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    /// <summary>first draft for triangle, as a stand-alone object for now.</summary>
    public class Triangle : IPrimitive
    {
        private readonly Vector3 _edgeAB;
        private readonly Vector3 _edgeAC;
        private readonly IMaterial _material;
        private readonly Vector3 _normal; // precomputed geometric outward normal
        private readonly Vector3 _vertexA;

        public Triangle(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, IMaterial material)
        {
            _vertexA = vertexA;
            _edgeAB = vertexB - vertexA;
            _edgeAC = vertexC - vertexA;
            _normal = Vector3.Normalize(Vector3.Cross(_edgeAB, _edgeAC));
            _material = material;
        }

        public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
        {
            var t0 = 0.0f;

            if (!TriangleIntersection(ray, _vertexA, _edgeAB, _edgeAC, rayInterval, ref t0).HasValue)
                return false;

            hitRecord.RayT = t0;
            hitRecord.HitPoint = ray.PointsAt(t0);
            hitRecord.Material = _material;
            hitRecord.SetFaceNormal(ray, _normal);

            return true; // hit at t0
        }

        public BoundingBox GetBoundingBox(float t0, float t1)
        {
            var vb = _vertexA + _edgeAB;
            var vc = _vertexA + _edgeAC;
            var min = Vector3.Min(Vector3.Min(_vertexA, vb), vc);
            var max = Vector3.Max(Vector3.Max(_vertexA, vb), vc);
            return new BoundingBox(min, max);
        }

        /// <summary>
        /// triangle intersection, refactored out for reuse in mesh triangle
        /// </summary>
        /// <param name="ray">incoming ray</param>
        /// <param name="vertexA">vertex A of triangle</param>
        /// <param name="edgeAB">edge a to vertex b</param>
        /// <param name="edgeAC">edge a to vertex c</param>
        /// <param name="rayInterval">ray interval</param>
        /// <param name="tRay">potential intersection at t</param>
        /// <returns>barycentric coordinates triple if hit, null otherwise</returns>
        public static (float u, float v, float w)? TriangleIntersection(in Ray ray,
            in Vector3 vertexA,
            in Vector3 edgeAB,
            in Vector3 edgeAC,
            in Interval rayInterval,
            ref float tRay)
        {
            var pVec = Vector3.Cross(ray.Direction.AsVector3(), edgeAC);
            var det = Vector3.Dot(edgeAB, pVec);

            if (MathF.Abs(det) < 1e-4f) // parallel to triangle plane
                return null;

            var invDet = 1.0f / det;

            var tVec = ray.Origin - vertexA;
            var u = Vector3.Dot(tVec, pVec) * invDet;
            if (u is < 0.0f or > 1.0f) return null;
            var qVec = Vector3.Cross(tVec, edgeAB);
            var v = Vector3.Dot(ray.Direction.AsVector3(), qVec) * invDet;
            if (v < 0.0f || v + u > 1.0f) return null;
            var t0 = Vector3.Dot(qVec, edgeAC) * invDet;

            // if (t0 < tMin || tMax < t0) return null;
            if (!rayInterval.Contains(t0)) return null;
            tRay = t0;
            return (u, v, 1.0f - (u + v));
        }
    }
}