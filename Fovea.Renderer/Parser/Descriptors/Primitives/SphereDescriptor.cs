using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; } = Vector3.Zero;
    public Vector3? Center1 { get; init; }
    [JsonIgnore]
    public bool IsMoving => Center1.HasValue;
    public required float Radius { get; init; }

    public IPrimitive Generate(ParserContext context)
    {
        var material = GetMaterial(context.Materials);

        return IsMoving
            ? new Sphere(Center, Center1!.Value, Radius, material)
            : new Sphere(Center, Radius, material);
    }
}