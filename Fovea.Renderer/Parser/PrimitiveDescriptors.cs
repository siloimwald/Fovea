using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser.Yaml;
using Fovea.Renderer.Primitives;
using YamlDotNet.Serialization;

namespace Fovea.Renderer.Parser;

public abstract class PrimitiveDescriptorBase
{
    [JsonPropertyName("material")]
    public string MaterialReference { get; init; } = string.Empty;

    protected IMaterial GetMaterialOrFail(IDictionary<string, IMaterial> materials)
    {
        if (materials.TryGetValue(MaterialReference, out var material))
        {
            return material;
        }

        throw new SceneReferenceNotFoundException(
            $"{this.GetType().Name} did not find referenced material {MaterialReference}");
    }
}

public class MeshProducerBase : PrimitiveDescriptorBase
{
    public bool FlipNormals { get; init; }

    /// <summary>
    /// produce MeshTriangles instead of normal once, required for UV support
    /// </summary>
    public bool AsMesh { get; init; }
}

/// <summary>
/// quad/parallelogram
/// </summary>
public class QuadDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Point { get; init; }
    public Vector3 AxisU { get; init; }
    public Vector3 AxisV { get; init; }

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        var material = GetMaterialOrFail(materials);
        return [new Quad(Point, AxisU, AxisV, material)];
    }
}

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; }
    public Vector3? Center1 { get; init; }
    [YamlIgnore]
    public bool IsMoving => Center1.HasValue;
    public float Radius { get; init; }

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        var material = GetMaterialOrFail(materials);
        // this is where C# 12 comes in handy ;)
        return
        [
            IsMoving
                ? new Sphere(Center, Center1!.Value, Radius, material)
                : new Sphere(Center, Radius, material)
        ];
    }
}

/// <summary>
/// take a primitive and flip its normals
/// </summary>
public class FlipFaceDescriptor : IPrimitiveGenerator
{
    public IPrimitiveGenerator Primitive { get; set; }

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        // not great, not terrible
        var innerPrim = Primitive.Generate(materials, context);
        return innerPrim.Select(p => new FlipFace(p)).ToList<IPrimitive>();
    }
}

public class MeshFileDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    /// <summary>
    /// obj file
    /// </summary>
    public string FileName { get; set; }

    public bool FlipNormals { get; set; }

    /// <summary>
    /// transform whole mesh into unit cube
    /// </summary>
    public bool Normalize { get; set; }

    /// <summary>
    /// generate per vertex normals, i.e. smoothed mesh
    /// </summary>
    public bool VertexNormals { get; set; }

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        // TODO: pass some context so this is relative to the scene file location, not the cwd
        var mesh = ObjReader.ReadObjFile(Path.Combine(context.SceneFileLocation, FileName), Normalize);
        return mesh.CreateMeshTriangles(GetMaterialOrFail(materials), FlipNormals, VertexNormals);
    }
}