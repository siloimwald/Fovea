using System.IO;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public class MeshFileDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    /// <summary>
    /// obj file
    /// </summary>
    public required string FileName { get; init; }

    public bool FlipNormals { get; init; } = false;

    /// <summary>
    /// transform whole mesh into unit cube
    /// </summary>
    public bool Normalize { get; init; } = false;

    /// <summary>
    /// generate per vertex normals, i.e. smoothed mesh
    /// </summary>
    public bool VertexNormals { get; init; } = false;

    public IPrimitive Generate(ParserContext context)
    {
        var mesh = ObjReader.ReadObjFile(Path.Combine(context.SceneFileLocation, FileName), Normalize);
        var prims = mesh.CreateMeshTriangles(GetMaterial(context.Materials), FlipNormals, VertexNormals);
        return new BVHTree(prims);
    }
}