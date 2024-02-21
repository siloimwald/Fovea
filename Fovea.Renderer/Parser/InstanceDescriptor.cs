using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Parser;


[JsonDerivedType(typeof(TranslationDescriptor), "translate")]
[JsonDerivedType(typeof(ScalingDescriptor), "scale")]
[JsonDerivedType(typeof(RotationDescriptor), "rotate")]
public interface ITransformationDescriptor
{
    Matrix4x4 GetTransformationMatrix();
}

public class TranslationDescriptor : ITransformationDescriptor
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public Matrix4x4 GetTransformationMatrix() => Matrix4x4.CreateTranslation(X, Y, Z);
}

public class ScalingDescriptor : ITransformationDescriptor
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public Matrix4x4 GetTransformationMatrix() => Matrix4x4.CreateScale(X, Y, Z);
}

public class RotationDescriptor : ITransformationDescriptor
{
    [JsonPropertyName("by")] // degree
    public float Angle { get; init; } = 0f;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Axis Axis { get; init; } = Axis.X;
    
    /// <summary>
    /// rotation center
    /// </summary>
    public Vector3 Center { get; init; } = Vector3.Zero;

    public Matrix4x4 GetTransformationMatrix()
    {
        var rad = MathUtils.DegToRad(Angle);
        // add center of rotation later
        return Axis switch
        {
            Axis.X => Matrix4x4.CreateRotationX(rad, Center),
            Axis.Y => Matrix4x4.CreateRotationY(rad, Center),
            _ => Matrix4x4.CreateRotationZ(rad, Center)
        };
    }
}


public static class TransformationListExtensions
{
    public static Matrix4x4 GetTransformation(this IEnumerable<ITransformationDescriptor> transforms)
    {
        return transforms.Aggregate(Matrix4x4.Identity, 
            (acc, transform) => acc * transform.GetTransformationMatrix());
    }
}

public class InstanceDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public List<ITransformationDescriptor> Transforms { get; init; } = [];
    [JsonPropertyName("blueprint")]
    public string BlueprintName { get; init; } = string.Empty;
    public List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context)
    {
        var forwardMatrix = Transforms.GetTransformation();
        Matrix4x4.Invert(forwardMatrix, out var inverse);
        if (!context.Blueprints.TryGetValue(BlueprintName, out var blueprint))
        {
            throw new SceneReferenceNotFoundException($"missing referenced blueprint {BlueprintName}");
        }

        return [new Instance(blueprint, forwardMatrix, inverse, GetMaterialOrFail(materials))];
    }
}