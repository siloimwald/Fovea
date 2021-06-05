using System;
using System.Runtime.CompilerServices;
using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives
{
    public class MeshTriangle : IPrimitive
    {
        private readonly TriangleMesh _mesh;
        private readonly int _faceIndex;

        public MeshTriangle(TriangleMesh mesh, int faceIndex)
        {
            _mesh = mesh;
            _faceIndex = faceIndex;
        }
        
        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var (f0, f1, f2) = _mesh.Faces[_faceIndex];
            var (va, vb, vc) = (_mesh.Vertices[f0], _mesh.Vertices[f1], _mesh.Vertices[f2]);
            var t0 = 0.0;

            var barycentricCoords = Triangle.TriangleIntersection(ray, va, vb - va, vc - va, tMin, tMax, ref t0);
            if (!barycentricCoords.HasValue)
                return false;

            var (u, v, w) = barycentricCoords.Value;
            
            hitRecord.RayT = t0;
            hitRecord.HitPoint = ray.PointsAt(t0);
            hitRecord.Material = _mesh.Material;

            if (_mesh.HasVertexNormals)
            {
                var na = _mesh.Normals[f0];
                var nb = _mesh.Normals[f1];
                var nc = _mesh.Normals[f2];
                var n = na * w + nb * u + nc * v;
                hitRecord.SetFaceNormal(ray, n);
            }
            else
            {
                hitRecord.SetFaceNormal(ray, _mesh.Normals[_faceIndex]);    
            }

            if (_mesh.Texture == null) return true; // hit at t0
            
            hitRecord.TextureU = _mesh.Texture[f0].texU * w + _mesh.Texture[f1].texU * u + _mesh.Texture[f2].texU * v;
            hitRecord.TextureV = _mesh.Texture[f0].texV * w + _mesh.Texture[f1].texV * u + _mesh.Texture[f2].texV * v;
            
            return true; // hit at t0
        }

        public BoundingBox GetBoundingBox(double t0, double t1)
        {
            var (f0, f1, f2) = _mesh.Faces[_faceIndex];
            var (va, vb, vc) = (_mesh.Vertices[f0], _mesh.Vertices[f1], _mesh.Vertices[f2]);
            return new BoundingBox(
                Point3.Min(va, Point3.Min(vb, vc)),
                Point3.Max(va, Point3.Max(vb, vc))
            );
        }

        public double PdfValue(Point3 origin, Vec3 direction)
        {
            var hitRecord = new HitRecord();
            if (!Hit(new Ray(origin, direction), 1e-4, double.PositiveInfinity, ref hitRecord))
                return 0.0;
            GetVertices(out var va, out var vb, out var vc);
            var edgeAB = vb - va;
            var edgeAC = vc - va;
            var area = 0.5 * Vec3.Cross(edgeAB, edgeAC).Length();
            var distanceSquared = (hitRecord.HitPoint - origin).LengthSquared();
            var cosine = Math.Abs(Vec3.Dot(direction, hitRecord.Normal) / direction.Length());
            var pdfVal = distanceSquared / (cosine * area);
            return pdfVal;
        }
        
        public Vec3 RandomDirection(Point3 origin)
        {
            var r1 = Math.Sqrt(Sampler.Instance.Random01());
            var r2 = Sampler.Instance.Random01();
            GetVertices(out var va, out var vb, out var vc);
            var w = 1.0 - r1;
            var v = r1 * (1.0 - r2);
            var u = r2 * r1;
            var p = va * w + vb * u + vc * v;
            return p - origin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetVertices(out Point3 va, out Point3 vb, out Point3 vc)
        {
            var (f0, f1, f2) = _mesh.Faces[_faceIndex];
            (va, vb, vc) = (_mesh.Vertices[f0], _mesh.Vertices[f1], _mesh.Vertices[f2]);
        }
    }
}