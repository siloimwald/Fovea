using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser;

public abstract class PrimitiveDescriptorBase
{
    [JsonPropertyName("material")]
    public string MaterialReference { get; init; } = string.Empty;

    protected IMaterial GetMaterialOrFail(IDictionary<string, IMaterial> materials)
    {
        // blueprint
        if (materials == null)
            return null;
        
        if (materials.TryGetValue(MaterialReference, out var material))
        {
            return material;
        }

        throw new SceneReferenceNotFoundException(
            $"{this.GetType().Name} did not find referenced material {MaterialReference}");
    }
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

public class BoxDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 PointA { get; init; }
    public Vector3 PointB { get; init; }
    
    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        return [BoxProducer.MakeBox(PointA, PointB, GetMaterialOrFail(materials))];
    }
}

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; }
    public Vector3? Center1 { get; init; }
    [JsonIgnore]
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


public class MeshFileDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    /// <summary>
    /// obj file
    /// </summary>
    public string FileName { get; init; }

    public bool FlipNormals { get; init; }

    /// <summary>
    /// transform whole mesh into unit cube
    /// </summary>
    public bool Normalize { get; init; }

    /// <summary>
    /// generate per vertex normals, i.e. smoothed mesh
    /// </summary>
    public bool VertexNormals { get; init; }

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        var mesh = ObjReader.ReadObjFile(Path.Combine(context.SceneFileLocation, FileName), Normalize);
        return mesh.CreateMeshTriangles(GetMaterialOrFail(materials), FlipNormals, VertexNormals);
    }
}

