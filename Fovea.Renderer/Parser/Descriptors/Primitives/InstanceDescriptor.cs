using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Descriptors.Transforms;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public class InstanceDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public List<ITransformationDescriptor> Transforms { get; init; } = [];
    [JsonPropertyName("blueprint")] public required string BlueprintName { get; init; } = string.Empty;

    public bool UseParentMaterial { get; init; }

    public IPrimitive Generate(ParserContext context)
    {
        var forwardMatrix = Transforms.GetTransformation();
        Matrix4x4.Invert(forwardMatrix, out var inverse);
        if (!context.Blueprints.TryGetValue(BlueprintName, out var blueprint))
        {
            throw new SceneReferenceNotFoundException($"missing referenced blueprint {BlueprintName}");
        }

        return new Instance(blueprint, forwardMatrix, inverse,
            GetMaterial(context.Materials), UseParentMaterial);
    }
}