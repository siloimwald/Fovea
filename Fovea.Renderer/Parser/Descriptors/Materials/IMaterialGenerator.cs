using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;

namespace Fovea.Renderer.Parser.Descriptors.Materials;

[JsonDerivedType(typeof(MatteDescriptor), "matte")]
[JsonDerivedType(typeof(MetalDescriptor), "metal")]
[JsonDerivedType(typeof(DielectricDescriptor), "glass")]
[JsonDerivedType(typeof(DiffuseLightDescriptor), "diffuseLight")]
public interface IMaterialGenerator
{
    IMaterial Generate(IDictionary<string, ITexture> textures);
}