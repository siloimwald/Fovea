
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser.Yaml;
using Fovea.Renderer.Primitives;
using YamlDotNet.Serialization;

namespace Fovea.Renderer.Parser;

public abstract class PrimitiveDescriptorBase
{
    [YamlMember(Alias = "material")]
    public string MaterialReference { get; init; } = string.Empty;

    protected IMaterial GetMaterialOrFail(IDictionary<string, IMaterial> materials)
    {
        if (materials.TryGetValue(MaterialReference, out var material))
        {
            return material;
        }

        throw new SceneReferenceNotFoundException($"{this.GetType().Name} did not find referenced material {MaterialReference}");
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
/// axis aligned quad (producing two triangles)
/// </summary>
public class QuadDescriptor : MeshProducerBase, IPrimitiveGenerator
{
    public Vector2 ExtentMin { get; init; }
    public Vector2 ExtentMax { get; init; }
    public float Position { get; init; }
    public string Axis { get; init; } = "Y";

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        var axis = Axis.ToUpper() switch
        {
            "X" => VectorMath.Axis.X,
            "Y" => VectorMath.Axis.Y,
            _ => VectorMath.Axis.Z
        };

        var min = Vector2.Min(ExtentMin, ExtentMax);
        var max = Vector2.Max(ExtentMin, ExtentMax);
        var material = GetMaterialOrFail(materials);
        
        var mesh = QuadProducer.Produce(min.X, max.X, min.Y, max.Y, Position, axis);
        return AsMesh ? mesh.CreateMeshTriangles(material, FlipNormals)
            : mesh.CreateSingleTriangles(material, FlipNormals);
    }
}

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; }
    public float Radius { get; init; }

    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        // this is where C# 12 comes in handy ;)
        return [new Sphere(Center, Radius, GetMaterialOrFail(materials))];
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