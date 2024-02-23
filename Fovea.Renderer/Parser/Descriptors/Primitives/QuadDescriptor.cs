using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

/// <summary>
/// quad/parallelogram
/// </summary>
public class QuadDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Point { get; init; } = Vector3.Zero;
    public Vector3 AxisU { get; init; } = Vector3.UnitX;
    public Vector3 AxisV { get; init; } = Vector3.UnitY;

    public IPrimitive Generate(ParserContext context)
    {
        var material = GetMaterial(context.Materials);
        return new Quad(Point, AxisU, AxisV, material);
    }
}