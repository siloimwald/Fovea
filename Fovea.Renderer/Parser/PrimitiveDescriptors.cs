
using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
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

/// <summary>
/// axis aligned quad (producing two triangles)
/// </summary>
public class QuadDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector2 TopLeft { get; init; }
    public Vector2 BottomRight { get; init; }
    public float Position { get; init; }
    public string Axis { get; init; } = "Y";


    public void Generate(IDictionary<string, IMaterial> materials, List<IPrimitive> existingPrimitives)
    {
        var axis = Axis.ToUpper() switch
        {
            "X" => VectorMath.Axis.X,
            "Y" => VectorMath.Axis.Y,
            _ => VectorMath.Axis.Z
        };

        var min = Vector2.Min(TopLeft, BottomRight);
        var max = Vector2.Max(TopLeft, BottomRight);
        var material = GetMaterialOrFail(materials);
        // TODO: might move the producer here
        existingPrimitives.AddRange(
            QuadProducer
                .Produce(min.X, max.X, min.Y, max.Y, Position, axis).CreateSingleTriangles(material));
    }
}

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; }
    public float Radius { get; init; }

    public void Generate(IDictionary<string, IMaterial> materials, List<IPrimitive> existingPrimitives)
    {
        existingPrimitives.Add(new Sphere(Center, Radius, GetMaterialOrFail(materials)));
    }
}