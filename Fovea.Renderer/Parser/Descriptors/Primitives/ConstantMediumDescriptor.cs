using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Descriptors.Materials;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public class ConstantMediumDescriptor : MaterialDescriptorBase, IPrimitiveGenerator
{
    public required float Density { get; init; }
    public required IPrimitiveGenerator Boundary { get; init; }
    public string Id => Boundary.Id;

    public IPrimitive Generate(ParserContext context)
    {
        var boundary = Boundary.Generate(context);
        return new ConstantMedium(boundary, Density, GetTextureOrFail(context.Textures));
    }
}