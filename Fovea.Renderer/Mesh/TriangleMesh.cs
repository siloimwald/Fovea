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
        public List<Vec3> Normals { get; private set; }
        public List<(int f0, int f1, int f2)> Faces { get; init; }
        public IMaterial Material { get; private set; }
        
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

        /// <summary>
        /// creates memory saving triangles which draw their vertices during intersection from the mesh itself
        /// </summary>
        /// <param name="material">material to use</param>
        /// <param name="flipNormals">changes winding order to flip the normals of each face</param>
        /// <returns></returns>
        public List<IPrimitive> CreateMeshTriangles(IMaterial material, bool flipNormals = false)
        {
            Material = material;
            var triangles = new List<IPrimitive>(Faces.Count);
            for (var i = 0; i < Faces.Count; i++)
            {
                triangles.Add(new MeshTriangle(this, i));
            }

            // for the time being just do per face normals
            if (Normals == null || Normals.Count != Faces.Count)
                GenerateNormals(flipNormals);
            
            return triangles;
        }

        private void GenerateNormals(bool flipNormals = false)
        {
            Normals = Faces.Select(face =>
            {
                var va = Vertices[face.f0];
                var vb = Vertices[flipNormals ? face.f2 : face.f1];
                var vc = Vertices[flipNormals ? face.f1 : face.f2];
                return Vec3.Normalize(Vec3.Cross(vb - va, vc - va));
            }).ToList();
        }

        public TriangleMesh ApplyTransform(Matrix4 matrix)
        {
            for (var i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = matrix * Vertices[i];
            }

            return this;
        }
        
        
    }
}