using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public class CylinderDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public float Min { get; init; } = -1;
    public float Max { get; init; } = 1;
    public float Radius { get; init; } = 1;
    
    public IPrimitive Generate(ParserContext context)
    {
        return new Cylinder(Min, Max, Radius, GetMaterial(context.Materials));
    }
}