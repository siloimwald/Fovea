using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Yaml;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Parser;

[JsonDerivedType(typeof(SphereDescriptor), "sphere")]
[JsonDerivedType(typeof(QuadDescriptor), "quad")]
public interface IPrimitiveGenerator
{
    List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context);
}