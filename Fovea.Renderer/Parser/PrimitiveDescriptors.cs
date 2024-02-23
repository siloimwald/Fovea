using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;
using Microsoft.Extensions.Logging;

namespace Fovea.Renderer.Parser;

public abstract class PrimitiveDescriptorBase
{
    private static readonly ILogger<PrimitiveDescriptorBase> Log = Logging.GetLogger<PrimitiveDescriptorBase>();
    
    [JsonPropertyName("material")]
    public string MaterialReference { get; init; } = string.Empty;

    protected IMaterial GetMaterial(IDictionary<string, IMaterial> materials)
    {
        var foundMaterial = materials.TryGetValue(MaterialReference, out var material);
        if (foundMaterial)
            return material;
        
        Log.LogWarning("material ({Reference}) not found for {Descriptor}",
            MaterialReference, GetType().Name);
        // or some pink drop in?
        return null;
    }
}


/// <summary>
/// quad/parallelogram
/// </summary>
public class QuadDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public required Vector3 Point { get; init; }
    public required Vector3 AxisU { get; init; }
    public required Vector3 AxisV { get; init; }

    public List<IPrimitive> Generate(ParserContext context)
    {
        var material = GetMaterial(context.Materials);
        return [new Quad(Point, AxisU, AxisV, material)];
    }
}

public class BoxDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public required Vector3 PointA { get; init; }
    public required Vector3 PointB { get; init; }
    
    public List<IPrimitive> Generate(ParserContext context)
    {
        return [BoxProducer.MakeBox(PointA, PointB, GetMaterial(context.Materials))];
    }
}

/// <summary>
/// groups children into own bvh structure
/// </summary>
public class SubNodeDescriptor : IPrimitiveGenerator
{
    public required List<IPrimitiveGenerator> Children { get; init; }
    
    public List<IPrimitive> Generate(ParserContext context)
    {
        var children = Children.SelectMany(c => c.Generate(context)).ToList();
        return [new BVHTree(children)];
    }
}

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public required Vector3 Center { get; init; }
    public Vector3? Center1 { get; init; }
    [JsonIgnore]
    public bool IsMoving => Center1.HasValue;
    public required float Radius { get; init; }

    public List<IPrimitive> Generate(ParserContext context)
    {
        var material = GetMaterial(context.Materials);
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

    public List<IPrimitive> Generate(ParserContext context)
    {
        var mesh = ObjReader.ReadObjFile(Path.Combine(context.SceneFileLocation, FileName), Normalize);
        return mesh.CreateMeshTriangles(GetMaterial(context.Materials), FlipNormals, VertexNormals);
    }
}

public class ConstantMediumDescriptor : MaterialDescriptorBase, IPrimitiveGenerator
{
    public float Density { get; init; }
    public IPrimitiveGenerator Boundary { get; init; }
    
    public List<IPrimitive> Generate(ParserContext context)
    {
        var boundary = Boundary.Generate(context);
        // TODO: fix this
        return [new ConstantMedium(boundary[0], Density, GetTextureOrFail(context.Textures))];
    }
}