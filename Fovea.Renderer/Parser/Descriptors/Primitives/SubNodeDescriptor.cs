using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

/// <summary>
/// groups children into own bvh structure
/// </summary>
public class SubNodeDescriptor : IPrimitiveGenerator
{
    public required List<IPrimitiveGenerator> Children { get; init; }
    
    public IPrimitive Generate(ParserContext context)
    {
        return new BVHTree(Children.Select(c => c.Generate(context)).ToList());
    }
}