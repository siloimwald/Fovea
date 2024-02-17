using System;
using System.Runtime.CompilerServices;
using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

public class MeshTriangle(TriangleMesh mesh, int faceIndex) : IPrimitive
{
    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        var (f0, f1, f2) = mesh.Faces[faceIndex];
        var (va, vb, vc) = (mesh.Vertices[f0], mesh.Vertices[f1], mesh.Vertices[f2]);
        var t0 = 0.0f;

        var barycentricCoords = Triangle.TriangleIntersection(ray, va, vb - va, vc - va, rayInterval, ref t0);
        if (!barycentricCoords.HasValue)
            return false;

        var (u, v, w) = barycentricCoords.Value;

        hitRecord.RayT = t0;
        hitRecord.HitPoint = ray.PointsAt(t0);
        hitRecord.Material = mesh.Material;

        if (mesh.HasVertexNormals)
        {
            var na = mesh.Normals[f0];
            var nb = mesh.Normals[f1];
            var nc = mesh.Normals[f2];
            var n = na * w + nb * u + nc * v;
            hitRecord.SetFaceNormal(ray, n);
        }
        else
        {
            hitRecord.SetFaceNormal(ray, mesh.Normals[faceIndex]);
        }

        if (mesh.Texture == null) return true; // hit at t0

        hitRecord.TextureU = mesh.Texture[f0].texU * w + mesh.Texture[f1].texU * u + mesh.Texture[f2].texU * v;
        hitRecord.TextureV = mesh.Texture[f0].texV * w + mesh.Texture[f1].texV * u + mesh.Texture[f2].texV * v;

        return true; // hit at t0
    }

    public BoundingBox GetBoundingBox(float t0, float t1)
    {
        var (f0, f1, f2) = mesh.Faces[faceIndex];
        var (va, vb, vc) = (mesh.Vertices[f0], mesh.Vertices[f1], mesh.Vertices[f2]);
        return new BoundingBox(
            Vector3.Min(va, Vector3.Min(vb, vc)),
            Vector3.Max(va, Vector3.Max(vb, vc))
        );
    }

    public float PdfValue(Vector3 origin, Vector3 direction)
    {
        var hitRecord = new HitRecord();
        if (!Hit(new Ray(origin, direction), Interval.HalfOpenWithOffset(), ref hitRecord))
            return 0.0f;
        GetVertices(out var va, out var vb, out var vc);
        var edgeAB = vb - va;
        var edgeAC = vc - va;
        var area = 0.5 * Vector3.Cross(edgeAB, edgeAC).Length();
        var distanceSquared = (hitRecord.HitPoint - origin).LengthSquared();
        var cosine = Math.Abs(Vector3.Dot(direction, hitRecord.Normal) / direction.Length());
        var pdfVal = distanceSquared / (cosine * area);
        return (float)pdfVal;
    }

    public Vector3 RandomDirection(Vector3 origin)
    {
        var r1 = (float)Math.Sqrt(Sampler.Instance.Random01());
        var r2 = Sampler.Instance.Random01();
        GetVertices(out var va, out var vb, out var vc);
        var w = 1.0f - r1;
        var v = r1 * (1.0f - r2);
        var u = r2 * r1;
        var p = va * w + vb * u + vc * v;
        return p - origin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GetVertices(out Vector3 va, out Vector3 vb, out Vector3 vc)
    {
        var (f0, f1, f2) = mesh.Faces[faceIndex];
        (va, vb, vc) = (mesh.Vertices[f0], mesh.Vertices[f1], mesh.Vertices[f2]);
    }
}