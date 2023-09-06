using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Parser
{
    /// <summary>
    ///     basic wavefront obj reading support. For now this ignores groups, normals and texture coordinates everything
    ///     within the file is assumed to be one single mesh. vertex indices are assumed to be not relative
    /// </summary>
    public static class ObjReader
    {
        public static TriangleMesh ReadObjFile(string fileName, bool normalize = false)
        {
            var vertices = new List<Point3>();
            var faces = new List<(int f0, int f1, int f2)>();

            try
            {
                var content = File.ReadAllLines(fileName);
                foreach (var line in content)
                {
                    // assume that the file plays nice and has no spaces in front
                    // nor extra spaces between the identifiers
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // vertex
                    if (line[0] == 'v')
                    {
                        var point =
                            // using linq just because we can.
                            line[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Select(p =>
                                {
                                    if (double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture,
                                        out var number))
                                        return number;

                                    return (double?) null;
                                })
                                .Where(p => p.HasValue)
                                .Select(p => p.Value)
                                .ToList();

                        if (point.Count == 3) // that seems to have worked out
                            vertices.Add(new Point3(point[0], point[1], point[2]));
                    }
                    else if (line[0] == 'f')
                    {
                        var fParts =
                            line[1..]
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Select(p =>
                                {
                                    // face is either v1 or v1/vt1 or v1/vt1/vn1 or v1//vn1
                                    // ignore everything except the vertex index for now
                                    var hasSlash = p.IndexOf('/');
                                    if (hasSlash >= 0)
                                        p = p[..hasSlash];

                                    if (int.TryParse(p, out var index))
                                        return index - 1; // indices are 1 based in the file
                                    return (int?) null;
                                })
                                .Where(p => p.HasValue)
                                .Select(p => p.Value)
                                .ToList();

                        if (fParts.Count == 3)
                            faces.Add((fParts[0], fParts[1], fParts[2]));
                    }
                }

                Console.WriteLine($"read {vertices.Count} vertices and {faces.Count} faces.");

                // almost done...
                // for easier scene placement, transform the whole mesh to the unit cube
                if (normalize)
                {
                    // this yells SIMD at you
                    var (min, max) = vertices.Aggregate(
                        (currentMin: new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
                            currentMax: new Vector3(float.NegativeInfinity, float.NegativeInfinity,
                                float.NegativeInfinity)),
                        (acc, p) => (Vector3.Min(acc.currentMin, p.AsVector3()), Vector3.Max(acc.currentMax, p.AsVector3())));

                    Console.WriteLine($"bounds {min} {max}");

                    var box = new BoundingBox(min, max);
                    // move things into origin by moving the inverse of the old centroid
                    var translate = box.GetCentroid();
                    // scale to [-1,1]
                    var scale = box.GetExtent();
                    // use the longest side as scale factor, this keeps ratios intact
                    var s = Math.Max(scale.X, Math.Max(scale.Y, scale.Z));

                    scale = new Vector3(2.0f / s, 2.0f / s, 2.0f / s);

                    vertices = vertices.Select(v =>
                    {
                        var inOrigin = v - translate.AsPoint3();
                        return new Point3(inOrigin.X * scale.X, inOrigin.Y * scale.Y, inOrigin.Z * scale.Z);
                    }).ToList();
                }

                return new TriangleMesh
                {
                    Vertices = vertices,
                    Faces = faces
                };
            }
            catch (Exception error)
            {
                Console.WriteLine($"failed to read file {fileName}");
                Console.WriteLine(error);
                return null;
            }
        }
    }
}