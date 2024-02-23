using Fovea.Renderer.Core;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public class BoxDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 PointA { get; init; } = Vector3.Zero;
    public Vector3 PointB { get; init; } = Vector3.One;
    
    public IPrimitive Generate(ParserContext context)
    {
        return BoxProducer.MakeBox(PointA, PointB, GetMaterial(context.Materials));
    }
}