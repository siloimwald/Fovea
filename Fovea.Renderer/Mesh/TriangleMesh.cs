using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Mesh
{
    public class TriangleMesh
    {
        public List<Point3> Vertices { get; init; }
        public List<(int f0,int f1,int f2)> Faces { get; init; }

        /// <summary>
        /// creates single triangles for this mesh, that is don't reuse any vertices but instead
        /// build every individual triangle. This increases storage but is somewhat faster to ray trace since
        /// you can precompute things
        /// </summary>
        /// <param name="material">material to use for all triangles</param>
        /// <returns></returns>
        public List<IPrimitive> CreateSingleTriangles(IMaterial material)
        {
            return 
                Faces
                    .Select(face => new Triangle(Vertices[face.f0], Vertices[face.f1], Vertices[face.f2], material))
                    .ToList<IPrimitive>();
        }
    }
}