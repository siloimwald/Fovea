using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public class DiskDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; } = Vector3.Zero;
    public Vector3 Normal { get; init; } = Vector3.UnitY;
    public float Radius { get; init; } = 1;
    
    public IPrimitive Generate(ParserContext context)
    {
        return new Disk(Center, Normal, Radius, GetMaterial(context.Materials));
    }
}