using System.Collections.Generic;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Mesh
{
    /// <summary>
    /// static factory for a box made of triangles
    /// </summary>
    public static class BoxProducer
    {
        /// <summary>
        /// produce a mesh for a box with the given extents
        /// </summary>
        /// <returns></returns>
        public static TriangleMesh Produce(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
        {
            (xMin, xMax) = xMin <= xMax ? (xMin, xMax) : (xMax, xMin);
            (yMin, yMax) = yMin <= yMax ? (yMin, yMax) : (yMax, yMin);
            (zMin, zMax) = zMin <= zMax ? (zMin, zMax) : (zMax, zMin);

            var vertices = new List<Point3>
            {
                // xy 'front' vertices
                new(xMin, yMin, zMin),
                new(xMax, yMin, zMin),
                new(xMax, yMax, zMin),
                new(xMin, yMax, zMin),
                // xy 'back' vertices
                new(xMin, yMin, zMax),
                new(xMax, yMin, zMax),
                new(xMax, yMax, zMax),
                new(xMin, yMax, zMax),
            };

            // winding order here should be fine now, (vb-va) x (vc-va) = outward normal
            var faces = new List<(int f0, int f1, int f2)>
            {
                // xy front
                (2, 1, 0), (3, 2, 0),
                // xy back
                (4, 5, 6), (4, 6, 7),
                // xz top .
                (6, 2, 3), (7, 6, 3),
                // xz bottom
                (0, 1, 5), (0, 5, 4),
                // yz left .
                (0, 4, 7), (0, 7, 3),
                // yz right .
                (6, 5, 1), (2, 6, 1)
            };

            return new TriangleMesh
            {
                Faces = faces,
                Vertices = vertices
            };
        }
    }
}