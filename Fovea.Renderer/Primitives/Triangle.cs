using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    /// <summary>
    /// first draft for triangle, as a stand-alone object for now.
    /// might want to have a mesh version later to reuse vertices and materials...
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
            var pVec = Vec3.Cross(ray.Direction, _edgeAC);
            var det = Vec3.Dot(_edgeAB, pVec);

            if (Math.Abs(det) < 1e-4) // parallel to triangle plane
                return false;

            var invDet = 1.0f / det;

            var tVec = ray.Origin - _vertexA;
            var u = Vec3.Dot(tVec, pVec) * invDet;
            if (u is < 0.0f or > 1.0f) return false;
            var qVec = Vec3.Cross(tVec, _edgeAB);
            var v = Vec3.Dot(ray.Direction, qVec) * invDet;
            if (v < 0.0f || v + u > 1.0f) return false;
            var t0 = Vec3.Dot(qVec, _edgeAC) * invDet;

            if (t0 <= tMin || tMax < t0) return false;

            hitRecord.RayT = t0;
            hitRecord.HitPoint = ray.PointsAt(t0);
            hitRecord.Material = _material;
            hitRecord.Normal = _normal;

            return true; // hit at t0
        }

        public BoundingBox GetBoundingBox()
        {
            var vb = _vertexA + _edgeAB;
            var vc = _vertexA + _edgeAC;
            var min = Point3.Min(Point3.Min(_vertexA, vb), vc);
            var max = Point3.Max(Point3.Max(_vertexA, vb), vc);
            return new BoundingBox(min, max);
        }
    }
}