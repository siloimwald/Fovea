using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

[JsonDerivedType(typeof(SphereDescriptor), "sphere")]
[JsonDerivedType(typeof(QuadDescriptor), "quad")]
[JsonDerivedType(typeof(BoxDescriptor), "box")]
[JsonDerivedType(typeof(MeshFileDescriptor), "meshFile")]
[JsonDerivedType(typeof(InstanceDescriptor), "instance")]
[JsonDerivedType(typeof(ConstantMediumDescriptor), "constantMedium")]
[JsonDerivedType(typeof(SubNodeDescriptor), "subNode")]
[JsonDerivedType(typeof(DiskDescriptor), "disk")]
[JsonDerivedType(typeof(CylinderDescriptor), "cylinder")]
public interface IPrimitiveGenerator
{
    IPrimitive Generate(ParserContext context);
}