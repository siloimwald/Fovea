using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser;

[JsonDerivedType(typeof(SphereDescriptor), "sphere")]
[JsonDerivedType(typeof(QuadDescriptor), "quad")]
[JsonDerivedType(typeof(BoxDescriptor), "box")]
public interface IPrimitiveGenerator
{
    List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context);
}