using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser;

[JsonDerivedType(typeof(SphereDescriptor), "sphere")]
[JsonDerivedType(typeof(QuadDescriptor), "quad")]
[JsonDerivedType(typeof(BoxDescriptor), "box")]
[JsonDerivedType(typeof(MeshFileDescriptor), "meshFile")]
[JsonDerivedType(typeof(InstanceDescriptor), "instance")]
[JsonDerivedType(typeof(ConstantMediumDescriptor), "constantMedium")]
[JsonDerivedType(typeof(SubNodeDescriptor), "subNode")]
public interface IPrimitiveGenerator
{
    List<IPrimitive> Generate(ParserContext context);
}